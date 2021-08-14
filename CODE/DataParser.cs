using Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text.RegularExpressions;

namespace DataParsers
{    
    public abstract class DataParser : IDataParser
    {
        protected readonly DataTable SourceTable = null;
        public DataParser(DataTable SourceTable) {
            this.SourceTable = SourceTable;
        }

        // -- static --

        /// Console.WriteLine(Math.Round(Time2Minutes("02:37:31"), 2));	// 157.52	    
        public static double Time2Minutes(string sTime) // format: HH:mm:ss
        {
            if (string.IsNullOrEmpty(sTime)) return 0;
            var match = Regex.Match(sTime, @"(?<HH>\d{2}):(?<mm>\d{2}):(?<ss>\d{2})");
            if (match == null) return 0;
            return Convert.ToInt32(match.Groups["HH"].Value) * 60 + Convert.ToInt32(match.Groups["mm"].Value) + (Convert.ToDouble(match.Groups["ss"].Value) / 60);
        }

        /// Console.WriteLine(Minutes2Time(157.52F)); // 02:37:31
        public static string Minutes2Time(double Minutes) // format: HH:mm:ss
        {
            if (Minutes <= 0) return "00:00:00";
            var hh = ((int)(Minutes / 60)).ToString();
            if (hh.Length == 1) hh = $"0{hh}";
            Minutes = Minutes % 60;
            var mm = ((int)(Minutes)).ToString();
            if (mm.Length == 1) mm = $"0{mm}";
            Minutes = Minutes - (int)(Minutes);
            var ss = Math.Round(Minutes * 60).ToString();
            if (ss.Length == 1) ss = $"0{ss}";
            return $"{hh}:{mm}:{ss}";
        }

        // -- protected --

        protected int RowCount {
            get {
                return this.SourceTable?.Rows.Count ?? 0;
            }
        }

        protected int LastRowIndex {
            get {
                return this.RowCount - 1;
            }
        }

        protected DataRow GetRow(int Index) {
            return this.SourceTable?.Rows[Index];
        }

        protected DataRow GetRowBySearchValue(string SearchValue, int ColumnIndex)
        {
            if (this.SourceTable == null) return null;

            foreach (DataRow row in this.SourceTable.Rows)
                if (string.Equals(row[ColumnIndex].ToString()?.Trim(), SearchValue, StringComparison.OrdinalIgnoreCase))
                    return row;
            return null;
        }
        protected DataRow GetRowBySearchPrefix(string SearchPrefix, int ColumnIndex)
        {
            if (this.SourceTable == null) return null;

            foreach (DataRow row in this.SourceTable.Rows)
                if (row[ColumnIndex].ToString()?.Trim().StartsWith(SearchPrefix, StringComparison.OrdinalIgnoreCase) ?? false)
                    return row;
            return null;
        }

        protected List<DataRow> GetNonEmptyRows(int ColumnIndex, int RowStartIndex = 0)
        {
            if (this.SourceTable == null) return null;

            var result = new List<DataRow>();
            var index = 0;
            foreach (DataRow row in this.SourceTable.Rows) {
                if (index++ < RowStartIndex) continue; // skip
                if (!string.IsNullOrEmpty(row[ColumnIndex].ToString()?.Trim()))
                    result.Add(row);                
            }
            return result;
        }

        protected string GetValue(int RowIndex, int ColIndex) {
            return this.SourceTable?.Rows[RowIndex][ColIndex].ToString() ?? string.Empty;
        }

        protected int CountBySearchValue(string SearchValue, int ColumnIndex)
        {
            if (this.SourceTable == null) return 0;

            var matchCount = 0;
            foreach (DataRow row in this.SourceTable.Rows)
                if (string.Equals(row[ColumnIndex].ToString()?.Trim(), SearchValue, StringComparison.OrdinalIgnoreCase))
                    matchCount++;
            return matchCount;
        }

        
    }
}
