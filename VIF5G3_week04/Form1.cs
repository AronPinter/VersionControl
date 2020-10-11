﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;
using System.Reflection;
using System.Data.Entity.Migrations.Model;

namespace VIF5G3_week04
{
    public partial class Form1 : Form
    {
        RealEstateEntities context = new RealEstateEntities();
        List<Flat> Flats;

        Excel.Application x1App ;
        Excel.Workbook x1WB;
        Excel.Worksheet x1Sheet;
        public Form1()
        {
            InitializeComponent();
            LoadData();
            CreateExcel();
        }
        private void LoadData()
        {
            Flats = context.Flats.ToList();
        }
        private void CreateExcel() 
        {
            try
            {
                x1App = new Excel.Application();
                x1WB = x1App.Workbooks.Add(Missing.Value);
                x1Sheet = x1WB.ActiveSheet;

                CreateTable();

                x1App.Visible = true;
                x1App.UserControl = true;
            }
            catch (Exception ex)
            {
                string errMsg = string.Format("Error: {0}\nLine: {1}", ex.Message, ex.Source);
                MessageBox.Show(errMsg, "Error");

                x1WB.Close(false, Type.Missing, Type.Missing);
                x1App.Quit();
                x1WB = null;
                x1App = null;
            }
        }
        private void CreateTable()
        {
            int lastRowID = x1Sheet.UsedRange.Rows.Count;
            int lastColumnID = x1Sheet.UsedRange.Columns.Count;

            string[] headers = new string[] 
            {"Kód",
            "Eladó",
            "Oldal",
            "Kerület",
            "Lift",
            "Szobák száma",
            "Alapterület (m2)",
            "Ár (mFt)",
            "Négyzetméter ár (Ft/m2)"};

            for (int i = 0; i < headers.Length; i++)
            {
                x1Sheet.Cells[1, i+1] = headers[1];
            }

            object[,] values = new object[Flats.Count, headers.Length];
            int counter = 0;
            foreach (Flat f in Flats)
            {
                values[counter, 0] = f.Code;
                values[counter, 1] = f.Vendor;
                values[counter, 2] = f.Side;
                values[counter, 3] = f.District;
                if (f.Elevator)
                {
                    values[counter, 4] ="van";
                }
                else
                {
                    values[counter, 4] = "nincs";
                }
                
                values[counter, 5] = f.NumberOfRooms;
                values[counter, 6] = f.FloorArea;
                values[counter, 7] = f.Price;
                values[counter, 8] = "";
                counter++;
            }
            x1Sheet.get_Range(
            GetCell(2, 1),
            GetCell(1 + values.GetLength(0), values.GetLength(1))).Value2 = values;

            Excel.Range headerRange = x1Sheet.get_Range(GetCell(1, 1), GetCell(1, headers.Length));
            headerRange.Font.Bold = true;
            headerRange.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
            headerRange.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            headerRange.EntireColumn.AutoFit();
            headerRange.RowHeight = 40;
            headerRange.Interior.Color = Color.LightBlue;
            headerRange.BorderAround2(Excel.XlLineStyle.xlContinuous, Excel.XlBorderWeight.xlThick);

            Excel.Range tableRange = x1Sheet.get_Range(GetCell(1, 1), GetCell(lastRowID, lastColumnID));
            tableRange.BorderAround2(Excel.XlLineStyle.xlContinuous, Excel.XlBorderWeight.xlThick);
            Excel.Range firstColumnRange = x1Sheet.get_Range(GetCell(1, 1), GetCell(1, lastColumnID));
            firstColumnRange.Font.Bold = true;
            Excel.Range lastColumnRange = x1Sheet.get_Range(GetCell(1, lastColumnID), GetCell(lastRowID, lastColumnID));
            lastColumnRange.Interior.Color = Color.LightGreen;
            lastColumnRange.NumberFormat = "#,##0.00";


        }
        private string GetCell(int x, int y)
        {
            string ExcelCoordinate = "";
            int dividend = y;
            int modulo;

            while (dividend > 0)
            {
                modulo = (dividend - 1) % 26;
                ExcelCoordinate = Convert.ToChar(65 + modulo).ToString() + ExcelCoordinate;
                dividend = (int)((dividend - modulo) / 26);
            }
            ExcelCoordinate += x.ToString();

            return ExcelCoordinate;
        }
    }

}
