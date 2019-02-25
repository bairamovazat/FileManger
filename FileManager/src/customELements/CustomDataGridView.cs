﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FileManager
{
    public abstract class CustomDataGridView : DataGridView
    {
        private bool enableDragAndDrop = false;
        private bool mouseDownOnRow = false;
        public CustomDataGridView()
        {
            this.MouseMove += CustomMouseMove;
            this.MouseUp += CustomMouseUp;
            this.MouseUp += CustomSelectElement;
            this.MouseLeave += CustomMouseLeave;
            this.GotFocus += CustomInvokeGotFocus;
            this.LostFocus += CustomInvokeLostFocus;
            this.ClearSelection();
        }

        public void CustomInvokeGotFocus(object sender, EventArgs e)
        {
            //Focus style
        }

        public void CustomInvokeLostFocus(object sender, EventArgs e)
        {
            // lost focus 
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            int mouseHoverRowIndex = this.HitTest(e.X, e.Y).RowIndex;
            mouseDownOnRow = mouseHoverRowIndex != -1;
            //При нажатии на строчку не передаём дальше 
            if (!mouseDownOnRow)
            {
                base.OnMouseDown(e);
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            this.mouseDownOnRow = false;
            base.OnMouseUp(e);
        }

        public abstract List<string> CurrentSelectedFiles();

        private void CustomMouseMove(object sender, MouseEventArgs e)
        {
            if (!enableDragAndDrop && (e.Button & MouseButtons.Left) == MouseButtons.Left && mouseDownOnRow)
            {
                enableDragAndDrop = true;
                string[] files = this.CurrentSelectedFiles().ToArray();
                if (files.Length > 0)
                {
                    DataObject data = new DataObject(DataFormats.FileDrop, files);
                    data.SetData(DataFormats.StringFormat, files[0]);
                    DoDragDrop(data, DragDropEffects.Copy);
                }
            }
        }

        private void CustomSelectElement(object sender, MouseEventArgs e)
        {
            int index = this.HitTest(e.X, e.Y).RowIndex;
            if (index != -1)
            {
                if (Control.ModifierKeys != Keys.Control)
                {
                    this.ClearSelection();
                }
                DataGridViewRow row = this.Rows[index];
                row.Selected = !row.Selected;
            }
        }

        private void CustomMouseUp(object sender, MouseEventArgs e)
        {
            enableDragAndDrop = false;
        }

        private void CustomMouseLeave(object sender, EventArgs e)
        {
            enableDragAndDrop = false;
        }

        public void SetData(string[,] data)
        {
            this.DataSource = GetDataTable(data);
        }

        public void SetData(List<List<string>> data)
        {
            this.DataSource = GetDataTable(data);
        }

        public static DataTable GetDataTable(string[,] data)
        {

            DataTable table = new DataTable();

            if (data.GetLength(0) != 0)
            {
                for (int i = 0; i < data.GetLength(1); i++)
                {
                    table.Columns.Add();
                }
                for (int i = 0; i < data.GetLength(0); i++)
                {
                    table.Rows.Add(table.NewRow());
                    for (int j = 0; j < data.GetLength(1); j++)
                    {
                        table.Rows[i][j] = data[i, j];
                    }
                }
            }
            return table;
        }

        public static DataTable GetDataTable(List<List<string>> data)
        {

            DataTable table = new DataTable();

            if (data.Count != 0)
            {
                data.First().ForEach(element => table.Columns.Add());

                for (int i = 0; i < data.Count; i++)
                {
                    table.Rows.Add(table.NewRow());
                    for (int j = 0; j < data.First().Count; j++)
                    {
                        table.Rows[i][j] = data[i][j];
                    }
                }
            }

            return table;
        }

    }
}
