using System.Collections.Generic;

namespace ObjectDumping.Tests.Testdata
{
    public class ViewModelValidation
    {
        private readonly Dictionary<string, List<string>> errorMessages = new Dictionary<string, List<string>>();

        public ViewModelValidation Errors => this;

        // Indexer declaration.
        // If index is out of range, the temps array will throw the exception.
        public List<string> this[string propertyName]
        {
            get => this.errorMessages[propertyName];
            set => this.errorMessages[propertyName] = value;
        }
    }
}
