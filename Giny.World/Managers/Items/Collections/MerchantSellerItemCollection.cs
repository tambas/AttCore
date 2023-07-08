using Giny.ORM;
using Giny.Protocol.Messages;
using Giny.World.Managers.Entities.Merchants;
using Giny.World.Records.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Giny.World.Managers.Items.Collections
{
    public class MerchantSellerItemCollection : ItemCollection<MerchantItemRecord>
    {
        private CharacterMerchant Merchant
        {
            get;
            set;
        }

        public MerchantSellerItemCollection(CharacterMerchant merchant,IEnumerable<MerchantItemRecord> items) : base(items)
        {
            this.Merchant = merchant;
        }
        /*
         * In this case, the collection must be thread safe.
         */
        protected override IList<MerchantItemRecord> CreateContainer()
        {
            return new SynchronizedCollection<MerchantItemRecord>();
        }
        public override void OnItemQuantityChanged(MerchantItemRecord item, int quantity)
        {
            item.QuantitySold += quantity;
            item.UpdateElement();

            Merchant.SendExchangers(new ExchangeShopStockMovementUpdatedMessage()
            {
                objectInfo = item.GetObjectItemToSell(),
            });
        }
        public override void OnItemRemoved(MerchantItemRecord item)
        {
            item.QuantitySold += item.Quantity;
            item.Quantity = 0;
            item.UpdateElement();
        
            Merchant.SendExchangers(new ExchangeShopStockMovementRemovedMessage()
            {
                objectId = item.UId,
            
            });
        }
    }
}
