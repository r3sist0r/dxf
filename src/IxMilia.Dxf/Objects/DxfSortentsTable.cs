using IxMilia.Dxf.Collections;
using IxMilia.Dxf.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace IxMilia.Dxf.Objects
{
    public partial class DxfSortentsTable
    {
        public IList<Tuple<DxfEntity,uint>> EntitiesWithDrawOrder()
        {
            List<Tuple<DxfEntity, uint>> ret = new List<Tuple<DxfEntity, uint>>(SortItemsPointers.Count);
            Debug.Assert(SortItemsPointers.Count == EntitiesPointers.Count);
            for (int i=0; i<SortItemsPointers.Count; ++i)
            {
                DxfPointer ptr = SortItemsPointers.Pointers[i];
                ret.Add(new Tuple<DxfEntity, uint>(EntitiesPointers[i], ptr.Handle));
            }
            return ret;
        }

        internal override DxfObject PopulateFromBuffer(DxfCodePairBufferReader buffer)
        {
            var isReadyForSortHandles = false;
            var ownerHandleRead = false;
            while (buffer.ItemsRemain)
            {
                var pair = buffer.Peek();
                if (pair.Code == 0)
                {
                    break;
                }

                while (this.TrySetExtensionData(pair, buffer))
                {
                    pair = buffer.Peek();
                }

                switch (pair.Code)
                {
                    case 5:
                        if (isReadyForSortHandles)
                        {
                            SortItemsPointers.Pointers.Add(new DxfPointer(DxfCommonConverters.UIntHandle(pair.StringValue)));
                        }
                        else
                        {
                            ((IDxfItemInternal)this).Handle = DxfCommonConverters.UIntHandle(pair.StringValue);
                            isReadyForSortHandles = true;
                        }
                        break;
                    case 100:
                        isReadyForSortHandles = true;
                        break;
                    case 330:
                        if (ownerHandleRead == false)
                        {
                            ((IDxfItemInternal)this).OwnerHandle = DxfCommonConverters.UIntHandle(pair.StringValue);
                            ownerHandleRead = true;
                            isReadyForSortHandles = true;
                        }
                        break;
                    case 331:
                        EntitiesPointers.Pointers.Add(new DxfPointer(DxfCommonConverters.UIntHandle(pair.StringValue)));
                        isReadyForSortHandles = true;
                        break;
                    default:
                        if (!base.TrySetPair(pair))
                        {
                            ExcessCodePairs.Add(pair);
                        }
                        break;
                }

                buffer.Advance();
            }

            return PostParse();
        }
    }
}
