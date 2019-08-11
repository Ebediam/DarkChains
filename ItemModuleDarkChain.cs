using BS;

namespace DarkChains
{
    // This create an item module that can be referenced in the item JSON
    public class ItemModuleDarkChain : ItemModule
    {
        public override void OnItemLoaded(Item item)
        {
            base.OnItemLoaded(item);
            item.gameObject.AddComponent<ItemDarkChain>();
        }
    }
}
