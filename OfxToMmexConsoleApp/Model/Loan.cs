using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic;

namespace OfxToMmex.Model
{
    class Loan
    {
        // account details
        private int _id;
        private string _description;
        private int _accountId;
        // loan details
        private double _loanValue;
        private double _deposit;
        private double _ballonPayment;
        private double _term;
        private string _termUnit;
        private Microsoft.VisualBasic.DueDate _dueDate;
        // introductory offer details
        private double _introRate;
        private double _introTerm;
        private double _introTermUnit;
        // standard rate details
        private double _standardRate;

        //public static double Pmt(double Rate,double NPer,double PV,double FV,DueDate Due)
        public double introPmt()
        {

            return Financial.Pmt(_introRate / 12, _term, -_loanValue, _ballonPayment, _dueDate);
        }

        public double svrPmt()
        {
            // recalc the loan value - IMPT / Rate
            double newLoanValue = Financial.IPmt(_introRate, _introTerm, _term, -_loanValue);
            return Financial.Pmt(_standardRate / 12, _term - _introTerm, newLoanValue, _ballonPayment, _dueDate);
        }



    }
}
