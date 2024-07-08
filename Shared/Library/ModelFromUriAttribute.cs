using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Text.Json;

namespace SharedLibrary;

/// <summary>
/// Mark a model parameter transferred in the URI of a controller method.
/// </summary>
public class ModelFromUriAttribute : ModelBinderAttribute
{
    /// <summary>
    /// 
    /// </summary>
    public class Binder : IModelBinder
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="bindingContext"></param>
        /// <returns></returns>
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            try
            {
                /* Read raw JSON seriaalization from query. */
                var json = bindingContext.HttpContext.Request.Query[bindingContext.ModelName].ToString();
                if (string.IsNullOrEmpty(json)) return Task.CompletedTask;

                /* Use WebSam deserializer to create the model. */
                bindingContext.Result = ModelBindingResult.Success(JsonSerializer.Deserialize(json, bindingContext.ModelType, LibUtils.JsonSettings));
            }
            catch (Exception)
            {
                /* Ignore any error. */
            }

            return Task.CompletedTask;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public ModelFromUriAttribute() => BinderType = typeof(Binder);
}