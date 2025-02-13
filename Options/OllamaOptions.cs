using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VectorStoreRAG.Options
{
    /// <summary>
    /// Configuration for Ollama AI services.
    /// </summary>
    public class OllamaOptions
    {
        /// <summary>
        /// The endpoint where Ollama is running (e.g., http://localhost:11434).
        /// </summary>
        public string Endpoint { get; set; } = "http://localhost:11434";

        /// <summary>
        /// The name of the model to use (e.g., llama2).
        /// </summary>
        public string Model { get; set; } = "phi3:latest";
    }
}
