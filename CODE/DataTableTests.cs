﻿using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;

namespace TestConsole9
{
    public class DataTableTests
    {
        private static List<DataColumn> COLUMNS = new List<DataColumn> {
            new DataColumn("Column-1"),
            new DataColumn("Column-2"),
            new DataColumn("Column-3"),
            new DataColumn("Column-4")
        };

        public static void Test1()  // DataTable-2-CSV
        {
            var table = new DataTable();
            table.Columns.AddRange(COLUMNS.ToArray());
            table.Rows.Add("1x1", "1x2", "1x3", "1x4");
            table.Rows.Add("2x1", "2x2", "2x3", "2x4");
            table.Rows.Add("3x1", "3x2", "3x3", "3x4");
            table.Rows.Add("4x1", "4x2", "4x3", "4x4");
            table.ToCSV1("D:\\export1.csv");
        }

        public static void Test2()  // DataTable-2-CSV
        {
            var table = new DataTable();
            table.Columns.AddRange(COLUMNS.ToArray());
            table.Rows.Add("1x1", "1x2", "1x3", "1x4");
            table.Rows.Add("2x1", "2x2", "2x3", "2x4");
            table.Rows.Add("3x1", "3x2", "3x3", "3x4");
            table.Rows.Add("4x1", "4x2", "4x3", "4x4");
            table.ToCSV2("D:\\export2.csv");
        }
    }

    public static class Extensions
    {
        public static void ToCSV1(this DataTable me, string filePath) {
            using (var sw = new StreamWriter(filePath, false, Encoding.UTF8)) {

                // header
                for (var i = 0; i < me.Columns.Count; i++) {
                    sw.Write(me.Columns[i]);
                    if (i < me.Columns.Count - 1) sw.Write(",");
                }
                sw.Write(sw.NewLine);

                // rows
                foreach (DataRow dr in me.Rows) {
                    for (var i = 0; i < me.Columns.Count; i++) {
                        if (!Convert.IsDBNull(dr[i])) {
                            var value = dr[i].ToString();
                            if (value.Contains(',')) {
                                value = $"\"{value}\"";
                                sw.Write(value);
                            }
                            else
                                sw.Write(dr[i].ToString());
                        }
                        if (i < me.Columns.Count - 1) sw.Write(",");
                    }
                    sw.Write(sw.NewLine);
                }                
            }
        }

        public static void ToCSV2(this DataTable me, string filePath) {
            var data = new StringBuilder();

            // header
            for (int column = 0; column < me.Columns.Count; column++) {
                var columnName = me.Columns[column].ColumnName.ToString().Replace(",", ";");
                var lastColumn = column == me.Columns.Count - 1;
                if (!lastColumn) columnName = $"{columnName},";
                data.Append(columnName);
            }        
            data.Append(Environment.NewLine);

            // rows
            for (int row = 0; row < me.Rows.Count; row++) {
                for (int column = 0; column < me.Columns.Count; column++) {
                    var rowData = me.Rows[row][column].ToString().Replace(",", ";");
                    var lastColumn = column == me.Columns.Count - 1;
                    if (!lastColumn) rowData = $"{rowData},";
                    data.Append(rowData);
                }

                // handle last empty line 
                if (row != me.Rows.Count - 1) data.Append(Environment.NewLine);
            }

            // save
            using (var objWriter = new StreamWriter(filePath, false, Encoding.UTF8))
                objWriter.WriteLine(data);            
        }
    }
}