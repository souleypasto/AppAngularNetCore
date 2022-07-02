using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BaseApiController : Controller
    {
        protected readonly DataContext _context;

        public BaseApiController(DataContext context) {
            _context = context;
        } 
    }
}