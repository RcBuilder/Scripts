using Entities;
using System.Collections.Generic;
using System.Linq;

namespace BLL
{
    public static class FiltersExtensions
    {
        /*
        public static void LoadFilters<TFilter, TFactory>(this List<TFilter> me, IEnumerable<FilterMetaData> filtersMetaData, TFactory filtersFactory) 
            where TFactory: IFactory<FilterMetaData, TFilter> 
            where TFilter : IFilter<TFilter>
        {
            foreach (var filterMetaData in filtersMetaData)
            {
                var filter = filtersFactory.Produce(filterMetaData);
                if (filter == null) continue;
                me.Add(filter);
            }
        }
        */

        public static void LoadFilters(this List<IAuctionFilter> me, IEnumerable<FilterMetaData> filtersMetaData, AuctionFilterFactory filtersFactory) {
            foreach (var filterMetaData in filtersMetaData) {
                var filter = filtersFactory.Produce(filterMetaData);
                if (filter == null) continue;
                me.Add(filter);
            }
        }
        
        public static void LoadFilters(this List<IArtworkFilter> me, IEnumerable<FilterMetaData> filtersMetaData, ArtworkFilterFactory filtersFactory)
        {
            foreach (var filterMetaData in filtersMetaData)
            {
                var filter = filtersFactory.Produce(filterMetaData);
                if (filter == null) continue;
                me.Add(filter);
            }
        }

        public static void LoadGlobalFilters(this List<IAuctionFilter> me, AuctionFilterFactory filtersFactory)
        {
            var filters = new List<FilterMetaData> {
                new FilterMetaData { EntityName = "Auction", Name = "AuctionHTMLDecoderFilter" },
                new FilterMetaData { EntityName = "Auction", Name = "AuctionDateConverterFilter" },
                new FilterMetaData { EntityName = "Auction", Name = "AuctionCopyFromIfEmptyFilter", Properties= "{ source:'SaleNumber', dest:'Id' }" },
                new FilterMetaData { EntityName = "Auction", Name = "AuctionIdUniqueFilter" },                
                new FilterMetaData { EntityName = "Auction", Name = "AuctionPhotoHostPrefixFilter" },
                new FilterMetaData { EntityName = "Auction", Name = "AuctionPhotoFilter", Properties= "PhotoRaw" },
                new FilterMetaData { EntityName = "Auction", Name = "AuctionURLHostPrefixFilter" },
                /// TODO ->> temporary disabled
                ///new FilterMetaData { EntityName = "Auction", Name = "AuctionSkipByExpirationFilter" },                
                new FilterMetaData { EntityName = "Auction", Name = "AuctionSkipByCompletionFilter" },                   
            };

            me.AddRange(filters.Select(x => filtersFactory.Produce(x)));            
        }

        public static void LoadGlobalFilters(this List<IArtworkFilter> me, ArtworkFilterFactory filtersFactory)
        {
            var filters = new List<FilterMetaData> {
                new FilterMetaData { EntityName = "Artwork", Name = "ArtworkSkipInvalidFilter" },
                new FilterMetaData { EntityName = "Artwork", Name = "ArtworkHTMLDecoderFilter" },
                new FilterMetaData { EntityName = "Artwork", Name = "ArtworkPhotoFilter", Properties= "PhotosRaw" },
                new FilterMetaData { EntityName = "Artwork", Name = "ArtworkDimensionsFilter" },
                new FilterMetaData { EntityName = "Artwork", Name = "ArtworkUnitFilter" },
                new FilterMetaData { EntityName = "Artwork", Name = "ArtworkPriceFilter" },
                new FilterMetaData { EntityName = "Artwork", Name = "ArtworkPriceRangeFilter" },
                new FilterMetaData { EntityName = "Artwork", Name = "ArtworkCurrencyConverterFilter" },                
                new FilterMetaData { EntityName = "Artwork", Name = "ArtworkIsSignedFilter" },                
                new FilterMetaData { EntityName = "Artwork", Name = "ArtworkIsWithdrawnFilter" },     
                new FilterMetaData { EntityName = "Artwork", Name = "ArtworkArtistTypeFilter" },
                new FilterMetaData { EntityName = "Artwork", Name = "ArtworkCreatedCircaFilter" },
                new FilterMetaData { EntityName = "Artwork", Name = "ArtworkCreatedFix2DigitsFilter" },
                new FilterMetaData { EntityName = "Artwork", Name = "ArtworkCreatedFix2DigitsRangeFilter" },   
                new FilterMetaData { EntityName = "Artwork", Name = "ArtworkCreatedDeduplicationFilter" },   
                new FilterMetaData { EntityName = "Artwork", Name = "ArtworkCreatedValidationFilter" },  
                new FilterMetaData { EntityName = "Artwork", Name = "ArtworkEditionRemovalFilter" },  
            };

            me.AddRange(filters.Select(x => filtersFactory.Produce(x)));
        }

        public static void Reset(this List<IAuctionFilter> me)
        {
            me.ForEach(x => { x.Value = null; });
        }

        public static void Reset(this List<IArtworkFilter> me)
        {
            me.ForEach(x => { x.Value = null; });
        }
    }
}
