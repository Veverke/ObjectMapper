using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ObjectMapper.Types;

namespace ObjectMapper
{
    /// <summary>
    /// Maps public properties only.
    /// </summary>
    public class Mapper
    {
        public TDest Map<TSource, TDest>(TSource sourceObj, bool overrideWithDefaultValues = false, List<string> fieldsToIgnore = null, Dictionary<string, string> customMapping = null)
            where TDest : new()
            where TSource : class
        {
            TDest destObj = Activator.CreateInstance<TDest>();
            PropertyInfo[] destProperties = destObj.GetType().GetProperties(BindingFlags.Instance | BindingFlags.IgnoreCase | BindingFlags.Public);
            IEnumerable<String> destPropertiesNames = destProperties.Select(propertyInfo => propertyInfo.Name.ToLower());
            PropertyInfo[] sourceProperties = sourceObj.GetType().GetProperties(BindingFlags.Instance | BindingFlags.IgnoreCase | BindingFlags.Public);
            IEnumerable<String> sourcePropertiesNames = sourceProperties.Select(propertyInfo => propertyInfo.Name.ToLower());

            //1st check: same name properties
            IEnumerable<String> mutualPropertyNames = destPropertiesNames.Intersect(sourcePropertiesNames);
            IEnumerable<string> propertyNamesToMap = fieldsToIgnore != null ? mutualPropertyNames.Except(fieldsToIgnore) : mutualPropertyNames;
            
            foreach (String propertyNameToMap in propertyNamesToMap)
            {
                PropertyInfo sourceMatchingProperty = sourceProperties.FirstOrDefault(propertyInfo => propertyInfo.Name.ToLower() == propertyNameToMap);
                PropertyInfo destMatchingProperty = destProperties.FirstOrDefault(propertyInfo => propertyInfo.Name.ToLower() == propertyNameToMap);
                object sourceValue = sourceMatchingProperty.GetValue(sourceObj);

                if (!sourceMatchingProperty.PropertyType.IsValueType && sourceValue != null)
                {
                    sourceValue = DeepClone(sourceValue);
                }
                else if (overrideWithDefaultValues && sourceMatchingProperty.PropertyType.IsValueType && sourceValue == Activator.CreateInstance(sourceMatchingProperty.PropertyType))
                {
                    continue;
                }

                if(customMapping != null)
                {
                    string matchingMappingProperty = string.Empty;
                    if (customMapping.TryGetValue(propertyNameToMap, out matchingMappingProperty))
                    {
                        destMatchingProperty = destProperties.FirstOrDefault(propertyInfo => propertyInfo.Name.ToLower() == matchingMappingProperty.ToLower());
                    }
                }

                if (sourceMatchingProperty.PropertyType.Name != destMatchingProperty.PropertyType.Name)
                {
                    try
                    {
                        sourceValue = Convert.ChangeType(sourceValue, destMatchingProperty.PropertyType);
                    }
                    catch (Exception ex)
                    {
                        System.Console.WriteLine("Could not convert from type [{0}] to [{1}]. Value: [{2}]. SrcPrp: [{3}] DestPrp: [{4}].", sourceMatchingProperty.PropertyType.Name, destMatchingProperty.PropertyType.Name, sourceValue, sourceMatchingProperty.Name, destMatchingProperty.Name);
                    }
                }

                destMatchingProperty.SetValue(destObj, sourceValue);
            }

            return destObj;
        }

        public MappedObject<TDest> Map<TSource, TDest>(TSource sourceObj, Dictionary<string, List<string>> extractions) 
            where TDest : new() 
            where TSource : class
        {
            TDest mappedObject = Map<TSource, TDest>(sourceObj, overrideWithDefaultValues: false, fieldsToIgnore: extractions.Keys.ToList());
            PropertyInfo[] sourceProperties = sourceObj.GetType().GetProperties();
            IDictionary<string, object> extraData = new ExpandoObject() as IDictionary<string, object>;

            foreach(KeyValuePair<string, List<string>> extraction in extractions)
            {
                PropertyInfo propertyInfo = sourceProperties.FirstOrDefault(srcPropertyInfo => srcPropertyInfo.Name.ToLower() == extraction.Key.ToLower());
                object propertyValue = propertyInfo.GetValue(sourceObj);
                extraData = ExtractFields(propertyValue, extraction.Value);
                //extraData.Add(extraction.Key, ExtractFields(propertyValue, extraction.Value)); 
            }

            return new MappedObject<TDest> { RegularMapping = mappedObject, Extras = extraData };
        }

        private object DeepClone(object obj)
        {
            Type objType = obj.GetType();
            object clone = Activator.CreateInstance(objType);
            PropertyInfo[] properties = objType.GetProperties();
            
            foreach(PropertyInfo property in properties)
            {
                object sourceValue = property.GetValue(obj);

                if (!property.PropertyType.IsValueType && property.PropertyType.Name.ToLower() != "string")
                {
                    sourceValue = DeepClone(sourceValue);
                }

                property.SetValue(clone, sourceValue);
            }

            return clone;
        }

        private dynamic ExtractFields(object obj, List<string> fields)
        {
            Type objType = obj.GetType();
            IEnumerable<PropertyInfo> properties = objType.GetProperties().Where(property => fields.Contains(property.Name.ToLower()));
            IDictionary<string, object> newObject = new ExpandoObject() as IDictionary<string, object>;

            foreach(PropertyInfo property in properties)
            {
                newObject.Add(property.Name, property.GetValue(obj));
            }

            return newObject;
        }
    }
}
