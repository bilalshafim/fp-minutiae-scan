using FP_Engine.Interfaces;
using System.Collections.Generic;

namespace FP_Engine.Handlers
{

    // Interface for generating templates and adding meta data to the data object (ID, etc) and storing in DB and cache.
    public class SubjectHandler
    {
        // Calls TemplateHandler to generate templates, adds metadata, and calls DBHandler to store data
        // TODO: Implement logic to support all supported formats
        public void GenerateTemplatesAndStore(ITemplateHandler templateHandler, object dbObject, List<byte[]> data, string format)
        {
            // Convert data to template

            // Convert template to byteArray

            // Create data object
            
            // Add meta data

            // Store by calling DBHandler
        }

        // TODO: Implement conversion of data objects (which contains template byte array and meta data) to T<FingerprintTemplate>
        // Retrieves data objects from DB and converts to T<FingerprinTemplate> for matching
        // TODO: Implement logic to support all supported formats
        public void RetrieveAndConvertData(object dbObject, string format)
        {
            // Retrieve data from Db by calling DBHandler
        }
    }
}
