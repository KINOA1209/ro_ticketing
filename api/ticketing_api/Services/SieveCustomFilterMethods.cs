using System;
using System.Linq;
using System.Linq.Expressions;
using Sieve.Services;
using ticketing_api.Models;
using ticketing_api.Models.Views;

namespace ticketing_api.Services
{
    public class SieveCustomFilterMethods : ISieveCustomFilterMethods
    {
        public IQueryable<OrderView> Market(IQueryable<OrderView> source, string op, string[] values)
        {
            foreach (var value in values)
            {
                if (op == "@=")
                {
                    var result = source.Where(p => p.MarketId.Name.Contains(value));
                    return result;
                }

                if (op == "==")
                {
                    var result = source.Where(p => p.MarketId.Name == value);
                    return result;
                }
            }

            return source;
        }

        public IQueryable<TaxView> Market(IQueryable<TaxView> source, string op, string[] values)
        {
            foreach (var value in values)
            {
                if (op == "@=")
                {
                    var result = source.Where(t => t.MarketId.Name.Contains(value));
                    return result;
                }

                if (op == "==")
                {
                    var result = source.Where(t => t.MarketId.Name == value);
                    return result;
                }
            }

            return source;
        }

        public IQueryable<OrderView> RigLocationId(IQueryable<OrderView> source, string op, string[] values)
        {
            foreach (var value in values)
            {
                if (op == "@=")
                {
                    var result = source.Where(p => p.RigLocationId.Id.ToString() == value);
                    return result;
                }

                if (op == "==")
                {
                    var result = source.Where(p => p.RigLocationId.Id.ToString() == value);
                    return result;
                }
            }

            return source;
        }

        public IQueryable<OrderView> RigLocationName(IQueryable<OrderView> source, string op, string[] values)
        {
            foreach (var value in values)
            {
                if (op == "@=")
                {
                    var result = source.Where(p => p.RigLocationId.Name.Contains(value.ToUpper()));
                    return result;
                }

                if (op == "==")
                {
                    var result = source.Where(p => p.RigLocationId.Name.Equals(value.ToUpper()));
                    return result;
                }
            }

            return source;
        }

        public IQueryable<ShippingPaperView> Market(IQueryable<ShippingPaperView> source, string op, string[] values)
        {
            foreach (var value in values)
            {

                if (op == "@=")
                {
                    var result = source.Where(t => t.MarketId.Name.Contains(value));
                    return result;
                }

                if (op == "==")
                {
                    var result = source.Where(t => t.MarketId.Name == value);
                    return result;
                }
            }

            return source;
        }

        public IQueryable<TicketPaperView> Market(IQueryable<TicketPaperView> source, string op, string[] values)
        {
            foreach (var value in values)
            {

                if (op == "@=")
                {
                    var result = source.Where(t => t.MarketId.Name.Contains(value));
                    return result;
                }
                if (op == "==")
                {
                    var result = source.Where(t => t.MarketId.Name == value);
                    return result;
                }
            }

            return source;
        }

        public IQueryable<ProductCategory> Name(IQueryable<ProductCategory> source, string op, string[] values)
        {
            foreach (var value in values)
            {
                if (op == "@=")
                {
                    var result = source.Where(t => t.Name.ToUpper().Contains(value.ToUpper()));
                    return result;
                }

                if (op == "==")
                {
                    var result = source.Where(t =>
                        string.Equals(t.Name, value, StringComparison.OrdinalIgnoreCase));
                    return result;
                }
            }

            return source;
        }

        public IQueryable<Unit> Name(IQueryable<Unit> source, string op, string[] values)
        {
            foreach (var value in values)
            {
                if (op == "@=")
                {
                    var result = source.Where(t => t.Name.ToUpper().Contains(value.ToUpper()));
                    return result;
    }

                if (op == "==")
                {
                    var result = source.Where(t =>
                        string.Equals(t.Name, value, StringComparison.OrdinalIgnoreCase));
                    return result;
                }
            }
    
            return source;
        }

        public IQueryable<WellView> Name(IQueryable<WellView> source, string op, string[] values)
        {
            foreach (var value in values)
            {
                if (op == "@=")
                {
                    var result = source.Where(t => t.Name.ToUpper().Contains(value.ToUpper()));
                    return result;
                }

                if (op == "==")
                {
                    var result = source.Where(t =>
                        string.Equals(t.Name, value, StringComparison.OrdinalIgnoreCase));
                    return result;
                }
            }

            return source;
        }
    }
}
