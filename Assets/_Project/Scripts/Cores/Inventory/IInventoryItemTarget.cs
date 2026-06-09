public interface IInventoryItemTarget
{
    bool CanReceiveItem(ShelterItemData itemData);
    bool ApplyItem(ShelterItemData itemData);
    void SetItemPreview(bool active, ShelterItemData itemData);
}
