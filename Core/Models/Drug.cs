using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Models
{
    public class Drug
    {
        public string Name { get; set; }
        public uint Id { get; set; }
        //Неизвестно, может ли быть несколько ДВ
        public Dictionary<uint,string> ActiveIngridients { get; set; }
        //Из задания не совсем понятно, какие именно ноды с уровнем связи 1621 нужно брать, потому я беру все
        public Dictionary<uint,string> AnatomicalTherapeuticChemicalClassification { get; set; }

    }
}
