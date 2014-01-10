/*
 * Code for the moment borrowed from http://stackoverflow.com/questions/13302933/how-to-avoid-firing-observablecollection-collectionchanged-multiple-times-when-r
 * */

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PCG_GUI
{
    public class SmartObservableCollection<T> : ObservableCollection<T>
    {
        public SmartObservableCollection()
            : base()
        {
        }

        public SmartObservableCollection(IEnumerable<T> collection)
            : base(collection)
        {
        }

        public SmartObservableCollection(List<T> list)
            : base(list)
        {
        }

        public void AddRange(IEnumerable<T> range)
        {
            foreach (var item in range)
            {
                Items.Add(item);
            }

            this.OnPropertyChanged(new PropertyChangedEventArgs("Count"));
            this.OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));
            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public void Reset(IEnumerable<T> range)
        {
            this.Items.Clear();

            AddRange(range);
        }
    }
    
}
