using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using C = ObjectMapper.Infra.Constants;

namespace ObjectMapper.Types
{


    /// <summary>
    ///     Maps public members of with get and setters (properties) only.
    ///     <remarks>
    ///         Requires that the destination and nested object types have a parameterless constructor.
    ///         When specifying custom mappings, you can specify one-to-one mappings, or use "contains" as the source property name, providing the string to search for as the matching property name.
    ///     </remarks>
    /// </summary>
    public class Mapper
    {
        public TDest Map<TSource, TDest>(TSource sourceObj, bool overrideWithDefaultValues = false, List<string> fieldsToIgnore = null, Dictionary<string, List<string>> customMappings = null)
            where TDest : new()
            where TSource : class
        {
            TDest destObj = Activator.CreateInstance<TDest>();
            PropertyInfo[] destProperties = destObj.GetType().GetProperties(BindingFlags.Instance | BindingFlags.IgnoreCase | BindingFlags.Public);
            IEnumerable<String> destPropertiesNames = destProperties.Select(propertyInfo => propertyInfo.Name);
            PropertyInfo[] sourceProperties = sourceObj.GetType().GetProperties(BindingFlags.Instance | BindingFlags.IgnoreCase | BindingFlags.Public);
            IEnumerable<String> sourcePropertiesNames = sourceProperties.Select(propertyInfo => propertyInfo.Name);
            IEnumerable<PropertyInfo> propertiesMarkedWithMappingAttribute = sourceProperties.Where(s => ((Attribute)s.GetCustomAttribute(typeof(MappingAttribute)) != null));

            //First thing, find same name properties
            IEnumerable<String> mutualPropertyNames = destPropertiesNames.Intersect(sourcePropertiesNames);
            List<MappingPropertyNames> propertiesNamesToMap = (fieldsToIgnore != null ? mutualPropertyNames.Except(fieldsToIgnore) : mutualPropertyNames).Select(propertyName => new MappingPropertyNames { SourceName = propertyName }).ToList();
            if (customMappings != null)
            {
                foreach (KeyValuePair<string, List<string>> customMapping in customMappings)
                {
                    //if there is a matching property with the same name, and a custom mapping for it is specified, map according to the custom mapping
                    MappingPropertyNames mappingPropertyNames = propertiesNamesToMap.FirstOrDefault(p => StringEquals(p.SourceName, customMapping.Key));

                    if (mappingPropertyNames != null)
                    {
                        mappingPropertyNames.CustomMappingNames.AddRange(customMapping.Value);
                    }
                    else
                    {
                        propertiesNamesToMap.Add(new MappingPropertyNames { SourceName = customMapping.Key, CustomMappingNames = customMapping.Value });
                    }
                }
            }

            if (propertiesMarkedWithMappingAttribute != null && propertiesMarkedWithMappingAttribute.Count() > 0)
            {
                foreach (PropertyInfo propertyInfo in propertiesMarkedWithMappingAttribute)
                {
                    MappingPropertyNames mappingPropertyNames = propertiesNamesToMap.FirstOrDefault(p => StringEquals(p.SourceName, propertyInfo.Name));
                    List<string> mappings = ((MappingAttribute)propertyInfo.GetCustomAttribute(typeof(MappingAttribute))).Matches;

                    if (mappingPropertyNames != null)
                    {
                        mappingPropertyNames.CustomMappingNames.AddRange(mappings);
                    }
                    else
                    {
                        propertiesNamesToMap.Add(new MappingPropertyNames { SourceName = propertyInfo.Name, CustomMappingNames = mappings });
                    }
                }
            }

            foreach (MappingPropertyNames propertyNamesToMap in propertiesNamesToMap)
            {
                PropertyInfo sourceMatchingProperty = GetMatchingProperties(sourceProperties, propertyNamesToMap, true, C.StringComparisionSetting).FirstOrDefault();
                List<PropertyInfo> destMatchingProperties = GetMatchingProperties(destProperties, propertyNamesToMap, true, C.StringComparisionSetting);

                //if after all no mapping property was determined, skip, instead of aborting
                if (destMatchingProperties.Count == 0)
                {
                    continue;
                }

                object sourceValue = sourceMatchingProperty.GetValue(sourceObj);

                if (!sourceMatchingProperty.PropertyType.IsValueType && !StringEquals(sourceMatchingProperty.Name, C.StringTypeName) && sourceValue != null)
                {
                    sourceValue = DeepClone(sourceValue);
                }
                else if (overrideWithDefaultValues && sourceMatchingProperty.PropertyType.IsValueType && sourceValue == Activator.CreateInstance(sourceMatchingProperty.PropertyType))
                {
                    continue;
                }

                foreach (PropertyInfo destProperty in destMatchingProperties)
                {
                    if (sourceMatchingProperty.PropertyType.Name != destProperty.PropertyType.Name)
                    {
                        try
                        {
                            sourceValue = Convert.ChangeType(sourceValue, destProperty.PropertyType);
                        }
                        catch (Exception ex)
                        {
                            System.Console.WriteLine("Could not convert from type [{0}] to [{1}]. Value: [{2}]. SrcPrp: [{3}] DestPrp: [{4}].", sourceMatchingProperty.PropertyType.Name, destProperty.PropertyType.Name, sourceValue, sourceMatchingProperty.Name, destProperty.Name);
                        }
                    }

                    destProperty.SetValue(destObj, sourceValue);
                }
            }

            return destObj;
        }

        public MappedObject<TDest> Map<TSource, TDest>(TSource sourceObj, Dictionary<string, List<string>> subSet)
            where TDest : new()
            where TSource : class
        {
            TDest mappedObject = Map<TSource, TDest>(sourceObj, overrideWithDefaultValues: false, fieldsToIgnore: subSet.Keys.ToList());
            PropertyInfo[] sourceProperties = sourceObj.GetType().GetProperties();
            IDictionary<string, object> extraData = new ExpandoObject() as IDictionary<string, object>;

            foreach (KeyValuePair<string, List<string>> extraction in subSet)
            {
                PropertyInfo propertyInfo = sourceProperties.FirstOrDefault(srcPropertyInfo => srcPropertyInfo.Name.ToLower() == extraction.Key.ToLower());
                object propertyValue = propertyInfo.GetValue(sourceObj);
                extraData = ExtractFieldSubset(propertyValue, extraction.Value);
            }

            return new MappedObject<TDest> { RegularMapping = mappedObject, Extras = extraData };
        }

        private object DeepClone(object obj)
        {
            Type objType = obj.GetType();
            object clone = Activator.CreateInstance(objType);
            PropertyInfo[] properties = objType.GetProperties();

            foreach (PropertyInfo property in properties)
            {
                object sourceValue = property.GetValue(obj);

                if (!property.PropertyType.IsValueType && property.PropertyType.Name.ToLower() != C.StringTypeName)
                {
                    sourceValue = DeepClone(sourceValue);
                }

                property.SetValue(clone, sourceValue);
            }

            return clone;
        }

        private dynamic ExtractFieldSubset(object obj, List<string> fieldSubSet)
        {
            Type objType = obj.GetType();
            IEnumerable<PropertyInfo> properties = objType.GetProperties().Where(property => fieldSubSet.Contains(property.Name.ToLower()));
            IDictionary<string, object> newObject = new ExpandoObject() as IDictionary<string, object>;

            foreach (PropertyInfo property in properties)
            {
                newObject.Add(property.Name, property.GetValue(obj));
            }

            return newObject;
        }

        private List<PropertyInfo> GetMatchingProperties(PropertyInfo[] propertiesCollection, MappingPropertyNames propertyNameToMap, bool overrideWithCustomMapping, StringComparison stringComparisionSetting = C.StringComparisionSetting)
        {
            List<PropertyInfo> matchingProperties = new List<PropertyInfo>();
            PropertyInfo matchingPropertyInfo = null;

            if (StringEquals(propertyNameToMap.SourceName, "contains"))
            {
                foreach (string nameMatch in propertyNameToMap.CustomMappingNames)
                {
                    matchingPropertyInfo = propertiesCollection.FirstOrDefault(propertyInfo => propertyInfo.Name.IndexOf(nameMatch, stringComparisionSetting) > -1);
                    if (matchingPropertyInfo != null)
                        matchingProperties.Add(matchingPropertyInfo);
                }
            }
            else
            {
                matchingPropertyInfo = propertiesCollection.FirstOrDefault(propertyInfo => StringEquals(propertyInfo.Name, propertyNameToMap.SourceName));
                if (matchingPropertyInfo != null)
                    matchingProperties.Add(matchingPropertyInfo);

                if (propertyNameToMap.CustomMappingNames.Count > 0)
                {
                    foreach (string nameMatch in propertyNameToMap.CustomMappingNames)
                    {
                        matchingPropertyInfo = propertiesCollection.FirstOrDefault(propertyInfo => propertyInfo.Name.IndexOf(nameMatch, stringComparisionSetting) > -1);
                        if (matchingPropertyInfo != null)
                            matchingProperties.Add(matchingPropertyInfo);
                    }
                }
            }

            if ((matchingPropertyInfo == null || overrideWithCustomMapping))
            {
                PropertyInfo previousValue = matchingPropertyInfo;

                foreach (string nameMatch in propertyNameToMap.CustomMappingNames)
                {
                    matchingPropertyInfo = propertiesCollection.FirstOrDefault(propertyInfo => nameMatch.IndexOf(propertyInfo.Name, stringComparisionSetting) > -1);
                    if (matchingPropertyInfo != null)
                        matchingProperties.Add(matchingPropertyInfo);
                }

                if (matchingPropertyInfo == null)
                {
                    matchingPropertyInfo = previousValue;
                }
            }

            return matchingProperties;
        }

        private bool StringEquals(string string1, string string2, StringComparison stringComparisionSetting = C.StringComparisionSetting)
        {
            return string1.Equals(string2, stringComparisionSetting);
        }
    }
}
