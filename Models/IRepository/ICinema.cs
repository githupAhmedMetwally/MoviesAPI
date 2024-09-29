﻿using Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.IRepository
{
	public interface ICinema:IGenericRepository<Cinema>
	{
		void Update(Cinema cinema);
	}
}