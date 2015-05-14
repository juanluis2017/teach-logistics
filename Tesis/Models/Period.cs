﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace Tesis.Models
{
    public class Period
    {
        public Guid Id { get; set; }

        public DateTime Created { get; set; }

        public bool IsLastPeriod { get; set; }

        [DisplayName("Sección")]
        public virtual Section Section { get; set; }

        public Guid SectionId { get; set; }

        [DisplayName("Demandas")]
        public virtual ICollection<Sale> Sales { get; set; }

        [DisplayName("Balances")]
        public virtual ICollection<Balance> Balances { get; set; }

        [DisplayName("Ordenes")]
        public virtual ICollection<Order> Orders { get; set; }
    }
}