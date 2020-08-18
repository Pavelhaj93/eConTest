namespace eContracting.Kernel.GlassItems.RenderingParameters.Modal_window
{
    using Glass.Mapper.Sc.Configuration.Attributes;

    [SitecoreType(TemplateId = "{0C212FAC-007E-4B30-A42E-44613D538683}", AutoMap = true)]
    public class MW01RenderingParameters
    {
        [SitecoreField]
        public virtual bool Centered_Content { get; set; }

        [SitecoreField]
        public virtual bool Has_Fixed_Width { get; set; }

        [SitecoreField]
        public virtual bool Initially_Opened { get; set; }

        [SitecoreField]
        public virtual bool IsFullWidth { get; set; }
    }
}
