using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace ProdentisApp
{
    class AppDataUtil
    {
        private static StringArray box(string str)
        {
            StringArray array = new StringArray();
            array.Insert(str);
            return array;
        }

        public static Patients processProdentis(DBUtil.DBResult result)
        {
            Patients Patients = new Patients();
            for (int i = 0; i < result.Count; i++)
            {
                List<string> row = result[i];
                Patient Patient = HL7Util.getEmptyPatient();
                HL7Util.addIdentifier(ref Patient, "Card", row[0]);
                HL7Util.setFirstName(ref Patient, row[1]);
                HL7Util.setSecondName(ref Patient, row[2]);
                HL7Util.setFamily(ref Patient, row[3]);
                if (row[8].Contains("1"))
                    HL7Util.setGender(ref Patient, "male"); 
                else
                    HL7Util.setGender(ref Patient, "female"); 
                HL7Util.addStreetPart(ref Patient, row[5]);
                HL7Util.addStreetPart(ref Patient, row[6]);
                HL7Util.addStreetPart(ref Patient, row[7]);
                HL7Util.setPostalCode(ref Patient, row[8]);
                HL7Util.setCity(ref Patient, row[9]);
                HL7Util.addIdentifier(ref Patient, "gmina", row[10]);
                HL7Util.addIdentifier(ref Patient, "miasto", row[11]);
                HL7Util.addIdentifier(ref Patient, "wojewodztwo", row[12]);
                HL7Util.setCountry(ref Patient, row[13]);
                HL7Util.setVatCode(ref Patient, row[14]);
                HL7Util.setPatientalCode(ref Patient, row[15]);
                HL7Util.setBirthDate(ref Patient, row[16].Replace("00:00:00",""));
                HL7Util.addIdentifier(ref Patient, "birthPlace", row[17]);
                HL7Util.addContact(ref Patient, "email", row[18]);
                HL7Util.addContact(ref Patient, "phone", row[19]);
                HL7Util.addContact(ref Patient, "phone", row[20]);
                HL7Util.addContact(ref Patient, "phone", row[21]);
                HL7Util.addIdentifier(ref Patient, "smsReceiver", row[22]);
                HL7Util.addIdentifier(ref Patient, "emailReceiver", row[23]);
                HL7Util.addIdentifier(ref Patient, "IdentityDocumentNumber", row[24]);
                HL7Util.addIdentifier(ref Patient, "IdentityDocumentType", "Paszport");
                HL7Util.addIdentifier(ref Patient, "CompanyNIP", row[25]);
                HL7Util.addIdentifier(ref Patient, "NfzDepartmentCode", row[26].Replace("R",""));
                Patients.Insert(Patient);
            }
            return Patients;
        }
    }
}