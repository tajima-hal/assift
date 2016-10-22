﻿using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;
using Shiftwork.Library;
using System.Diagnostics;

namespace Shiftwork.Payload
{
    public static class JobToShift
    {
        public static void Run(Excel.Workbook book)
        {
            book.Application.ScreenUpdating = false;
            book.Application.DisplayAlerts = false;
            MainForm._MainFormInstance.inProrgamUse = true;
            int Rows = 0, Columns = 0;   //Rowがy座標
            Excel.Worksheet jobsheet;// 操作中のアプリケーション
            Excel.Worksheet idvsheet;
            Excel.Sheets sheets;
            sheets = book.Worksheets;
            jobsheet = (Excel.Worksheet)sheets.get_Item(sheets.getSheetIndex("仕事シフト"));
            idvsheet = (Excel.Worksheet)sheets.get_Item(sheets.getSheetIndex("個人シフト"));
            Excel.Range current = idvsheet.Cells[1, 1];　　　//セル単体です
            string value;
            int cellCount = 1;
            Excel.Range wholeRange;
            Stopwatch sw = new Stopwatch();

            Excel.Range allJobRange = jobsheet.Cells[MainForm._MainFormInstance.startaddr_row, MainForm._MainFormInstance.startaddr_col];
            allJobRange = allJobRange.get_Resize(MainForm._MainFormInstance.jobtype + 10, 90 + 10);


            Excel.Range allIdvRange = idvsheet.Cells[4, 5];
            allIdvRange = allIdvRange.get_Resize(MainForm._MainFormInstance.jobtype + 10, 90 + 10);
            allIdvRange.UnMerge();
            allIdvRange.Clear();

            allIdvRange = idvsheet.Cells[1, 1];
            allIdvRange = allIdvRange.get_Resize(MainForm._MainFormInstance.jobtype + 10, 90 + 10);
            allIdvRange.Interior.ColorIndex = 2;
            //allIdvRange.ClearContents();
            //allIdvRange.UnMerge();

            string[,] allString = allJobRange.DeepToString();　//仕事シフトを入れる
            string[,] allIdvString = allIdvRange.DeepToString(); //個人シフトを入れる

            Excel.Range JobRange = jobsheet.Cells[MainForm._MainFormInstance.startaddr_row, MainForm._MainFormInstance.startaddr_col-1];
            JobRange = JobRange.get_Resize(MainForm._MainFormInstance.jobtype + 10, 1);
            string[,] jobString = JobRange.DeepToString();  //仕事名を入れる

            sw.Start();
            for (Rows = 0; Rows < MainForm._MainFormInstance.jobtype; Rows++)
            {
                for (Columns = 0; Columns < 100; Columns++)
                {
                    if (allString[Rows, Columns] == null || allString[Rows, Columns] == "")
                        continue;
                    else
                    {
                        for(int tmp=0; tmp < MainForm._MainFormInstance.jobtype; tmp++)
                        {

                            if(allIdvString[tmp,3]==null || allIdvString[tmp, 3] == "")
                            { }
                            else if (allString[Rows,Columns] == allIdvString[tmp,3] )
                            { 
                                allIdvString[tmp,Columns+4] = jobString[Rows, 0];
                                break;
                            }
                        }
                    }
                }
            }

            allIdvRange.set_Value(Type.Missing, allIdvString);

            book.Application.ScreenUpdating = true;
            book.Application.DisplayAlerts = false;
            for (Rows=1; Rows < MainForm._MainFormInstance.jobtype; Rows++)
            {
                for(Columns=1; Columns<100; Columns++)
                {
                    cellCount = 1;
                    value = allIdvString[Rows - 1, Columns - 1];
                    if (value == null || value == "")
                    { }
                    else
                    {
                        while (value == allIdvString[Rows - 1, Columns])
                        {
                            cellCount++;
                            Columns++;
                        }


                        wholeRange = idvsheet.Cells[Rows, Columns - cellCount + 1];
                        wholeRange = wholeRange.get_Resize(1, cellCount);
                        book.Application.DisplayAlerts = false;
                        wholeRange.Merge();
                        wholeRange.Interior.ColorIndex = 35;
                        wholeRange.BorderAround2();
                    }
                }
            }
            sw.Stop();



            MessageBox.Show(sw.ElapsedMilliseconds+"ミリ秒で処理が終了しました．\r\n");
            book.Application.ScreenUpdating = false;
            book.Application.DisplayAlerts = true;
            MainForm._MainFormInstance.inProrgamUse = false;
        }
    }
}
