using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Json.Orm.Extensions
{
    /// <summary>
    /// When custom attributes are created, this class helps 
    /// with the processing of them.
    /// </summary>
    public static class AttributeExtensions
    {

        /// <summary>This method will peer into an attributeItem and extract a specific 
        ///          generic class if it exists.</summary>    
        /// <param name="attributeItem">The target item.</param>    
        /// <returns>The decorated attribute or null</returns>    
        public static T ExtractAttribute<TargetType, T>(this TargetType attributeItem) where T : System.Attribute
        {
            T retVal = null;

            try
            {
                retVal = attributeItem?.GetType()
                        .GetProperty(attributeItem.ToString())
                        .ExtractAttribute<T>()
                    ;

            }
            catch (NullReferenceException)
            {
                //Occurs when we attempt to get description of an enum value that does not exist        
            }

            return retVal;
        }


        public static T ExtractAttribute<T>(this PropertyInfo propInfo) where T : System.Attribute
        {
            T retVal = null;

            try
            {
                retVal = propInfo?.GetCustomAttributes(typeof(T), false)
                    .OfType<T>()
                    .FirstOrDefault();

            }
            catch (NullReferenceException)
            {
                //Occurs when we attempt to get description of an enum value that does not exist        
            }

            return retVal;
        }

    }
}
