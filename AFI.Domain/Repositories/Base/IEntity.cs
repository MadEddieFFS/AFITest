﻿using System;
using System.Collections.Generic;
using System.Text;

namespace AFI.Domain.Repositories.Base
{
  public interface IEntity
    {
        public int Id { get; set; }
        public DateTime CreatedUTC { get; set; }
    }
}
