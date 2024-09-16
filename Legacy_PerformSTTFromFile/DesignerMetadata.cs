using System.Activities.Presentation.Metadata;
using System.ComponentModel;

namespace Legacy.PerformSTTFromFile
{
    public class DesignerMetadata : IRegisterMetadata
    {
        public void Register()
        {
            AttributeTableBuilder attributeTableBuilder = new AttributeTableBuilder();
            attributeTableBuilder.AddCustomAttributes(typeof(SpeechToTextActivity), new DesignerAttribute(typeof(ActivityDesignData)));
            MetadataStore.AddAttributeTable(attributeTableBuilder.CreateTable());
        }
    }
}