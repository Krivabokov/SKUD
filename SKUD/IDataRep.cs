using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SKUD
{
    public interface IDataRep
    {
        /*заполнение и удаление значений базы данных*/
        public bool AddPerson(string FIO, string Photo, string RFID);
        public bool RemovePerson(string FIO, string RFID);

        public bool AddDepartment(string Caption, string HeadName,
            string Telephone, string Email);
        public bool RemoveDepartment(string Caption);

        public bool AddPosition(string Caption);
        public bool RemovePosition(string Caption);

        public bool AddHeadOfDepartment(string Department, string Person);
        public bool RemoveHeadOfDepartment(string Department);

        public bool SetPositionsOfDepartment(string Department,
            List<string> ListOfPositions);

        public bool AddEmployee(string Person, string Department, string Position);
        public bool RemoveEmployee(string Person);

        public bool AddLog(string RFID, int In_Out, int check, int FaceControl);
        public bool RemoveLog(string StartDate, string EndDate);

        public List<string> GetDepartments();
        public List<string> GetPositions();
        public List<string> GetDataByRFID();
        public List<string> GetLog(string StartDate, string EndDate);

    }
}
