using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Model
{
    public enum RequestStatus
    {
        RequestSent = 1,
        RequestReceived = 2,
        AwaitingPaymentConfirmation = 3,
        RequestProcessed = 4,
        RequestDispatched = 5,
        RequestDelivered = 6
    }
}
