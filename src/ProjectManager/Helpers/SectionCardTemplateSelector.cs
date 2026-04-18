using System.Windows;
using System.Windows.Controls;
using ProjectManager.Models;

namespace ProjectManager.Helpers;

public class SectionCardTemplateSelector : DataTemplateSelector
{
    public DataTemplate? DescriptionTemplate { get; set; }
    public DataTemplate? PhotosTemplate      { get; set; }
    public DataTemplate? AttachmentsTemplate { get; set; }
    public DataTemplate? CustomTemplate      { get; set; }

    public override DataTemplate? SelectTemplate(object item, DependencyObject container)
        => item is SectionCard card ? card.Type switch
        {
            SectionCardType.Description  => DescriptionTemplate,
            SectionCardType.Photos       => PhotosTemplate,
            SectionCardType.Attachments  => AttachmentsTemplate,
            SectionCardType.Custom       => CustomTemplate,
            _                            => null
        } : null;
}
