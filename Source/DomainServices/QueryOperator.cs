namespace DomainServices
{
    using System.ComponentModel;

    /// <summary>
    /// Enum QueryOperator
    /// </summary>
    public enum QueryOperator
    {
        /// <summary>
        /// Greater than
        /// </summary>
        [Description(">")]
        GreaterThan,

        /// <summary>
        /// Greater than or equal
        /// </summary>
        [Description(">=")]
        GreaterThanOrEqual,

        /// <summary>
        /// Less than
        /// </summary>
        [Description("<")]
        LessThan,

        /// <summary>
        /// Less than or equal
        /// </summary>
        [Description("<=")]
        LessThanOrEqual,

        /// <summary>
        /// Like
        /// </summary>
        [Description("Like")]
        Like,

        /// <summary>
        /// Not like
        /// </summary>
        [Description("Not Like")]
        NotLike,

        /// <summary>
        /// Equal
        /// </summary>
        [Description("=")]
        Equal,

        /// <summary>
        /// Not equal
        /// </summary>
        [Description("!=")]
        NotEqual,

        /// <summary>
        /// Any
        /// </summary>
        [Description("Any")]
        Any,

        /// <summary>
        /// Intersects
        /// </summary>
        [Description("Intersects")]
        Intersects,

        /// <summary>
        /// Contains
        /// </summary>
        [Description("Contains")]
        Contains,

        /// <summary>
        /// SpatiallyIntersects
        /// </summary>
        [Description("Spatially Intersects")]
        SpatiallyIntersects,

        /// <summary>
        /// SpatiallyContains
        /// </summary>
        [Description("Spatially Contains")]
        SpatiallyContains,

        /// <summary>
        /// SpatiallyWithin
        /// </summary>
        [Description("Spatially Within")]
        SpatiallyWithin,

        /// <summary>
        /// SpatiallyWithinDistance
        /// </summary>
        [Description("Spatially Within Distance")]
        SpatiallyWithinDistance,

        /// <summary>
        /// SpatiallyTransform
        /// </summary>
        [Description("Spatially Transform")]
        SpatiallyTransform
    }
}