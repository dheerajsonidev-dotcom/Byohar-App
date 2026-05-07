namespace Byohar.Application.Mappings
{
    public static class ObjectMapper
    {
        public static void MapNonNullProperties<TSource, TDestination>(TSource source, TDestination destination)
        {
            if (source == null || destination == null)
            {
                throw new ArgumentNullException();
            }

            var sourceType = typeof(TSource);
            var destinationType = typeof(TDestination);

            foreach (var sourceProperty in sourceType.GetProperties())
            {
                var destinationProperty = destinationType.GetProperty(sourceProperty.Name);

                if (destinationProperty != null)
                {
                    var valueFromSource = sourceProperty.GetValue(source);

                    if (valueFromSource != null)
                    {
                        destinationProperty.SetValue(destination, valueFromSource);
                    }
                }
            }
        }
    }

}
