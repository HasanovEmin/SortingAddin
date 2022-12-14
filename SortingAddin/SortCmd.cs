using Aveva.ApplicationFramework.Presentation;
using Aveva.Core.Database;
using Aveva.Core.Database.Filters;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace SortingAddin
{
    class SortCmd : Command
    {
        DbElement ce;
        TypeFilter filter;
        List<Item> items;
        List<Item> sortedList;
        DbElementType type;         //Will be used for manipulating position
        public SortCmd()
        {
            base.Key = "Emin.SortingCmd";
            sortedList = new List<Item>();
            type = null;
        }

        public override void Execute()
        {

            SetTypeAndFilter();
            CollectComponents();
            Sort();
            ReplaceItems();
            
            base.Execute();
        }

        private void SetTypeAndFilter()
        {
            items = new List<Item>();
            ce = CurrentElement.Element;
            if (ce.GetActualType() == DbElementTypeInstance.PRTELE)
            {
                filter = new TypeFilter(DbElementTypeInstance.GPART);
                type = DbElementTypeInstance.GPART;

                DbElement[] collection = ce.Members();
                foreach (DbElement ele in collection)
                {
                    if (ele.GetActualType() != DbElementTypeInstance.GPART)
                    {
                        MessageBox.Show("PRTELE can contain only GPARTs");
                        return;
                    }
                }
            }
            else if (ce.GetActualType() == DbElementTypeInstance.CATEGORY)
            {
                filter = new TypeFilter(DbElementTypeInstance.SCOMPONENT);
                type = DbElementTypeInstance.SCOMPONENT;
            }
            else
            {
                MessageBox.Show("Can run only under PRTELE and CATE");
                return;
            }
        }

        private void ReplaceItems()
        {
            //creating pointer to first item by special type
            DbElement targetElement = ce.FirstMember(type);
            foreach (Item item in sortedList)
            {
                //if this item is first in collection, then we need to past it before current first item by type
                //then we need to replace target pointer
                if (item == sortedList[0])
                {
                    item.Element.InsertBefore(targetElement);
                    targetElement = item.Element;
                }
                else
                {
                    item.Element.InsertAfter(targetElement);
                    targetElement = item.Element;
                }
            }

            MessageBox.Show($"{ce.GetAsString(DbAttributeInstance.NAMN)} sorted");
        }

        /// <summary>
        /// Sorts with LINQ
        /// </summary>
        private void Sort()
        {
            sortedList = items.OrderBy(x => x.P1bore).ThenBy(x => x.P2bore).ThenBy(x =>x.P3bore).ThenBy(x => x.P4bore).ToList();
        }


        private void CollectComponents()
        {
            DBElementCollection collection = new DBElementCollection(ce, filter);
            foreach (DbElement element in collection)
            {
                DbQualifier qualifierP1 = new DbQualifier();
                qualifierP1.Add(1);
                DbQualifier qualifierP2 = new DbQualifier();
                qualifierP2.Add(2);
                DbQualifier qualifierP3 = new DbQualifier();
                qualifierP3.Add(3);
                DbQualifier qualifierP4 = new DbQualifier();
                qualifierP4.Add(4);

                double p1boreDouble = 0;
                double p2boreDouble = 0;
                double p3boreDouble = 0;
                double p4boreDouble = 0;

                element.GetValidDouble(DbAttributeInstance.PPBO, qualifierP1, ref p1boreDouble);
                element.GetValidDouble(DbAttributeInstance.PPBO, qualifierP2, ref p2boreDouble);
                element.GetValidDouble(DbAttributeInstance.PPBO, qualifierP3, ref p3boreDouble);
                element.GetValidDouble(DbAttributeInstance.PPBO, qualifierP4, ref p4boreDouble);
                items.Add(new Item()
                {
                    Element = element,
                    P1bore = p1boreDouble,
                    P2bore = p2boreDouble,
                    P3bore = p3boreDouble,
                    P4bore = p3boreDouble,
                });       
            }
        }
    }
}
