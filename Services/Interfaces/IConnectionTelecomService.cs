using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GoogleRuta.Models;

namespace GoogleRuta.Services.Interfaces;

public interface IConnectionTelecomService
{
    Task<ConnectionTelecom> ConnectionTelecom(ConnectionTelecom connectionTelecom);
    //Task<ConnectionTelecom> GetConnectionTelecom();
}