﻿using Balance.BackEnd.v2.Servicios.ActivosService.Modelos;
using Balance.BackEnd.v2.Servicios.MovimientosService.Modelos;
using System;

namespace Balance.BackEnd.v2.Servicios.ActivosService
{
    public class ActivosService : IActivosService
    {
        public async Task<Activos> GenerarActivos(List<Movimiento> movimientos)
        {
            Activos activos = new Activos();

            activos.Divisas = GenerarMonedas(movimientos);
            activos.Titulos = GenerarTitulos(movimientos);

            return await Task.FromResult(activos);
        }

        private List<Moneda> GenerarMonedas(List<Movimiento> movimientos)
        {
            if (movimientos == null || movimientos.Count == 0)
            {
                throw new ArgumentException("No se puede generar monedas sin movimientos");
            }

            List<Moneda> monedas = new List<Moneda>();

            foreach (Movimiento movimiento in movimientos)
            {

                Moneda? monedaExistente = monedas.FirstOrDefault(m => m.Tipo == movimiento.MontoTotal.Tipo);

                if (monedaExistente == null)
                {
                    Moneda nuevaMoneda = new Moneda { 
                        IdTipo = movimiento.MontoTotal.IdTipo,
                        Tipo = movimiento.MontoTotal.Tipo,
                        Cantidad = movimiento.MontoTotal.Cantidad,
                        Descripcion = movimiento.MontoTotal.Descripcion,
                        Simbolo = movimiento.MontoTotal.Simbolo
                    };

                    monedas.Add(nuevaMoneda);
                }
                else
                {
                    monedaExistente.Cantidad += movimiento.MontoTotal.Cantidad;
                }

            }

            return monedas;
        }

        private List<Titulo> GenerarTitulos(List<Movimiento> movimientos)
        {
            if (movimientos == null || movimientos.Count == 0)
            {
                throw new ArgumentException("No se puede generar titulos sin movimientos");
            }

            List<Titulo> titulos = new List<Titulo>();
            List<Movimiento> movimientosOrdenados = movimientos.OrderBy(m => m.FechaMovimiento).ToList();

            //TODO: Ver forma de sacar el tipo de titulo

            foreach (Movimiento movimiento in movimientosOrdenados)
            {
                Titulo? tituloExistente = titulos.FirstOrDefault(t => t.Ticket.TicketString == movimiento.Ticket.TicketString && t.Ticket.IdTipo == movimiento.Ticket.IdTipo);

                if (tituloExistente != null)
                {
                    if (movimiento.TipoMovimiento.Tipo == "COMPRA")
                    {
                        tituloExistente.Cantidad += movimiento.Cantidad;
                        tituloExistente.Historial.Add(movimiento);
                    }
                    //else if (movimiento.TipoMovimiento == Entidades.Enums.TipoMovimientoEnum.VENTA         //            Ver de resolver esto en una instancia anterior
                    //        && movimiento.Ticket == "AL30D"                                                // DOLAR MEP  tatando de identificar el dolar mep
                    //        && movimiento.MontoTotal.Tipo == Entidades.Enums.TipoMonedaEnum.PESO_ARG)      //            como otro tipo de movimiento
                    //{
                    //    titulos.First(t => t.Ticket == "AL30").Cantidad -= movimiento.Cantidad;
                    //    titulos.First(t => t.Ticket == "AL30D").Cantidad += movimiento.Cantidad;
                    //    tituloExistente.Historial.Add(movimiento);
                    //}
                    else if (movimiento.TipoMovimiento.Tipo == "VENTA")
                    {
                        tituloExistente.Cantidad -= movimiento.Cantidad;
                        tituloExistente.Historial.Add(movimiento);
                    }
                }
                else
                {
                    if (movimiento.TipoMovimiento.Tipo == "COMPRA")
                    {
                        titulos.Add(new Titulo()
                        {
                            Ticket = movimiento.Ticket,
                            Cantidad = movimiento.Cantidad,
                            Historial = new List<Movimiento> { movimiento }
                        });
                    }
                    else if (movimiento.TipoMovimiento.Tipo == "VENTA")
                    {
                        titulos.Add(new Titulo()
                        {
                            Ticket = movimiento.Ticket,
                            Cantidad = (movimiento.Cantidad * -1),
                            Historial = new List<Movimiento> { movimiento }
                        });
                    }
                }
            }

            return titulos;
        }
    }
}
