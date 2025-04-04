﻿using System;

namespace SPG_Fachtheorie.Aufgabe1.Services
{
    [Serializable]
    internal class PaymentServiceException : Exception
    {
        public PaymentServiceException()
        {
        }

        public PaymentServiceException(string? message) : base(message)
        {
        }

        public PaymentServiceException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}
