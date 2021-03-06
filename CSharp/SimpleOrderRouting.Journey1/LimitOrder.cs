// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LimitOrder.cs" company="LunchBox corp">
//     Copyright 2014 The Lunch-Box mob: 
//           Ozgur DEVELIOGLU (@Zgurrr)
//           Cyrille  DUPUYDAUBY (@Cyrdup)
//           Tomasz JASKULA (@tjaskula)
//           Mendel MONTEIRO-BECKERMAN (@MendelMonteiro)
//           Thomas PIERRAIN (@tpierrain)
//     
//     Licensed under the Apache License, Version 2.0 (the "License");
//     you may not use this file except in compliance with the License.
//     You may obtain a copy of the License at
//         http://www.apache.org/licenses/LICENSE-2.0
//     Unless required by applicable law or agreed to in writing, software
//     distributed under the License is distributed on an "AS IS" BASIS,
//     WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//     See the License for the specific language governing permissions and
//     limitations under the License.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace SimpleOrderRouting.Journey1
{
    using System;

    using SimpleOrderRouting.Interfaces;
    using SimpleOrderRouting.Interfaces.SmartOrderRouting;

    public class LimitOrder : IOrder
    {
        public LimitOrder(Market market, Way way, decimal price, int quantity, bool allowPartialExecution)
        {
            this.Market = market;
            this.AllowPartialExecution = allowPartialExecution;
            this.Way = way;
            this.Price = price;
            this.Quantity = quantity;
        }

        public event EventHandler<DealExecutedEventArgs> OrderExecuted;

        public event EventHandler<OrderFailedEventArgs> OrderFailed;

        public Market Market { get; private set; }

        public bool AllowPartialExecution { get; private set; }

        public Way Way { get; private set; }

        public decimal Price { get; set; }

        public int Quantity { get; private set; }

        public void Send()
        {
            this.Market.OrderExecuted += this.Market_OrderExecuted;
            EventHandler<string> marketOnOrderFailed = (sender, s) => this.RaiseOrderFailed(s);
            this.Market.OrderFailed += marketOnOrderFailed;
            this.Market.Send(this);
            this.Market.OrderExecuted -= this.Market_OrderExecuted;
            this.Market.OrderFailed -= marketOnOrderFailed;
        }

        private void RaiseOrderFailed(string s)
        {
            var onOrderFailed = this.OrderFailed;
            if (onOrderFailed != null)
            {
                onOrderFailed(this, new OrderFailedEventArgs(this.Market, s));
            }
        }

        private void Market_OrderExecuted(object sender, DealExecutedEventArgs e)
        {
            if (this == sender)
            {
                // we have been executed
                this.OnOrderExecuted(e);
            }
        }

        protected void OnOrderExecuted(DealExecutedEventArgs args)
        {
            var onOrderExecuted = this.OrderExecuted;
            if (onOrderExecuted != null)
            {
                onOrderExecuted(this, args);
            }
        }
    }
}