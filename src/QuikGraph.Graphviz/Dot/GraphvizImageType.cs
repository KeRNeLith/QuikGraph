#if SUPPORTS_DESCRIPTION
using System.ComponentModel;
#endif

namespace QuikGraph.Graphviz.Dot
{
    /// <summary>
    /// Enumeration of possible image types.
    /// </summary>
    public enum GraphvizImageType
    {
        /// <summary>
        /// Client side imagemaps.
        /// </summary>
#if SUPPORTS_DESCRIPTION
        [Description("Client-side imagemaps")]
#endif
        Cmap = 6,

        /// <summary>
        /// Figure format.
        /// </summary>
#if SUPPORTS_DESCRIPTION
        [Description("Figure format")]
#endif
        Fig = 0,

        /// <summary>
        /// Gd format.
        /// </summary>
#if SUPPORTS_DESCRIPTION
        [Description("Gd format")]
#endif
        Gd = 1,

        /// <summary>
        /// Gd2 format.
        /// </summary>
#if SUPPORTS_DESCRIPTION
        [Description("Gd2 format")]
#endif
        Gd2 = 2,

        /// <summary>
        /// GIF format.
        /// </summary>
#if SUPPORTS_DESCRIPTION
        [Description("GIF format")]
#endif
        Gif = 3,

        /// <summary>
        /// HP-GL/2 format.
        /// </summary>
#if SUPPORTS_DESCRIPTION
        [Description("HP-GL/2 format")]
#endif
        Hpgl = 4,

        /// <summary>
        /// Server-side imagemaps.
        /// </summary>
#if SUPPORTS_DESCRIPTION
        [Description("Server-side imagemaps")]
#endif
        Imap = 5,

        /// <summary>
        /// JPEG format.
        /// </summary>
#if SUPPORTS_DESCRIPTION
        [Description("JPEG format")]
#endif
        Jpeg = 7,

        /// <summary>
        /// FrameMaker MIF format.
        /// </summary>
#if SUPPORTS_DESCRIPTION
        [Description("FrameMaker MIF format")]
#endif
        Mif = 8,

        /// <summary>
        /// MetaPost.
        /// </summary>
#if SUPPORTS_DESCRIPTION
        [Description("MetaPost")]
#endif
        Mp = 9,

        /// <summary>
        /// PCL format.
        /// </summary>
#if SUPPORTS_DESCRIPTION
        [Description("PCL format")]
#endif
        Pcl = 10,

        /// <summary>
        /// PIC format.
        /// </summary>
#if SUPPORTS_DESCRIPTION
        [Description("PIC format")]
#endif
        Pic = 11,

        /// <summary>
        /// Plain text format.
        /// </summary>
#if SUPPORTS_DESCRIPTION
        [Description("Plain text format")]
#endif
        PlainText = 12,

        /// <summary>
        /// Portable Network Graphics format
        /// </summary>
#if SUPPORTS_DESCRIPTION
        [Description("Portable Network Graphics format")]
#endif
        Png = 13,

        /// <summary>
        /// PostScript.
        /// </summary>
#if SUPPORTS_DESCRIPTION
        [Description("Postscript")]
#endif
        Ps = 14,

        /// <summary>
        /// PostScript for PDF.
        /// </summary>
#if SUPPORTS_DESCRIPTION
        [Description("PostScript for PDF")]
#endif
        Ps2 = 15,

        /// <summary>
        /// Scalable Vector Graphics.
        /// </summary>
#if SUPPORTS_DESCRIPTION
        [Description("Scalable Vector Graphics")]
#endif
        Svg = 0x10,

        /// <summary>
        /// Scalable Vector Graphics, gzipped.
        /// </summary>
#if SUPPORTS_DESCRIPTION
        [Description("Scalable Vector Graphics, gzipped")]
#endif
        Svgz = 0x11,

        /// <summary>
        /// VRML.
        /// </summary>
#if SUPPORTS_DESCRIPTION
        [Description("VRML")]
#endif
        Vrml = 0x12,

        /// <summary>
        /// Visual Thought format.
        /// </summary>
#if SUPPORTS_DESCRIPTION
        [Description("Visual Thought format")]
#endif
        Vtx = 0x13,

        /// <summary>
        /// Wireless BitMap format.
        /// </summary>
#if SUPPORTS_DESCRIPTION
        [Description("Wireless BitMap format")]
#endif
        Wbmp = 20
    }
}