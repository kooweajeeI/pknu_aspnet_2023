using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Specialized;
namespace TodoItemApp.Models
{
    public class TodoItemsCollection : ObservableCollection<TodoItem>    // ILIst<TodoItem>, List<TodoItem>
    {
        public void CopyFrom(IEnumerable<TodoItem> todoItems)
        {
            this.Items.Clear();

            foreach (TodoItem item in todoItems)
            {
                this.Items.Add(item);   // 하나씩 다시추가
            }
            // 데이터 바뀜
            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }
    }
}