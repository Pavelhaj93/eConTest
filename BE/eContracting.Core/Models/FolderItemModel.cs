using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Glass.Mapper.Sc.Configuration.Attributes;

namespace eContracting.Models
{
    [ExcludeFromCodeCoverage]
    public class FolderItemModel<TChild> : BaseSitecoreModel
    {
        [SitecoreChildren]
        public virtual IEnumerable<TChild> Children { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="FolderItemModel{TChild}"/> class.
        /// </summary>
        public FolderItemModel()
        {
            this.Children = Enumerable.Empty<TChild>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FolderItemModel{TChild}"/> class.
        /// </summary>
        /// <param name="items">The items.</param>
        internal FolderItemModel(IEnumerable<TChild> items)
        {
            this.Children = items;
        }
    }
}
