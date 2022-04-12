using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace SkyrimAnimationChecker
{
    public class GridUtils
    {
        public static readonly DependencyProperty ColumnDefinitionsProperty =
            DependencyProperty.RegisterAttached("ColumnDefinitions", typeof(ColumnDefinitionCollection),
                                                typeof(GridUtils),
                                                new PropertyMetadata(default(ColumnDefinitionCollection),
                                                                        OnColumnDefinitionsChanged));

        private static void OnColumnDefinitionsChanged(DependencyObject d, DependencyPropertyChangedEventArgs ev)
        {
            var grid = (Grid)d;
            var oldValue = (ColumnDefinitionCollection)ev.OldValue;
            var newValue = (ColumnDefinitionCollection)ev.NewValue;
            grid.ColumnDefinitions.Clear();
            if (newValue != null)
                foreach (var cd in newValue)
                    //grid.ColumnDefinitions.Add(cd);
                    grid.ColumnDefinitions.Add(new ColumnDefinition { Width = cd.Width });
        }

        public static void SetColumnDefinitions(Grid element, ColumnDefinitionCollection value)
        {
            element.SetValue(ColumnDefinitionsProperty, value);
        }

        public static ColumnDefinitionCollection GetColumnDefinitions(Grid element)
        {
            return (ColumnDefinitionCollection)element.GetValue(ColumnDefinitionsProperty);
        }
    }

    public class ColumnDefinitionCollection : List<ColumnDefinition> { }
}
