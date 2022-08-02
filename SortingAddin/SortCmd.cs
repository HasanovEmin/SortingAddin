using Aveva.ApplicationFramework.Presentation;
using Aveva.Core.Database;
using Aveva.Core.Database.Filters;
using ComLine = Aveva.Core.Utilities.CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SortingAddin
{
    class SortCmd : Command
    {
        DbElement ce;
        TypeFilter filter;
        List<Item> items;
        List<Item> sortedList;
        DbElementType type;
        public SortCmd()
        {
            base.Key = "Emin.SortingCmd";
            sortedList = new List<Item>();
            type = null;
        }

        public override void Execute()
        {
            items = new List<Item>();
            ce = CurrentElement.Element;
            if (ce.GetActualType() == DbElementTypeInstance.PRTELE)
            {
                filter = new TypeFilter(DbElementTypeInstance.GPART);
                type = DbElementTypeInstance.GPART;

                DbElement[] col = ce.Members();
                foreach (DbElement ele in col)
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

            CollectComponents();
            Sort();
            ReplaceItems();
            
            base.Execute();
        }

        private void ReplaceItems()
        {
            DbElement targetElement = ce.FirstMember(type);
            foreach (Item item in sortedList)
            {
                if (sortedList[0] == item)
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

            MessageBox.Show($"{ce.GetAsString(DbAttributeInstance.NAME)} sorted");
        }

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
                double p3boreDouble = 0;
                double p4boreDouble = 0;
                element.GetValidDouble(DbAttributeInstance.PPBO, qualifierP3, ref p3boreDouble);
                element.GetValidDouble(DbAttributeInstance.PPBO, qualifierP4, ref p4boreDouble);
                items.Add(new Item()
                {
                    Element = element,
                    P1bore = element.GetDouble(DbAttributeInstance.PPBO, qualifierP1),
                    P2bore = element.GetDouble(DbAttributeInstance.PPBO, qualifierP2),
                    P3bore = p3boreDouble,
                    P4bore = p3boreDouble,
                });       
            }
        }
    }
}
