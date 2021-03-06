﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ConcentracionNotas.Models;
using Inacap.Infraestructura.Utilidad;

namespace ConcentracionNotas.Controllers
{
    /// <summary>
    /// hola
    /// </summary>
    public class AlumnoController : BaseController
    {
        private ModeloEntities db = new ModeloEntities();

        //
        // GET: /Alumno/prueba

        public ActionResult Index()
        {
            return View(db.Alumno.ToList());
        }

        //
        // GET: /Alumno/Details/5

        public ActionResult Details(int id = 0)
        {
            Alumno alumno = db.Alumno.Single(a => a.AlumnoId == id);
            if (alumno == null)
            {
                return HttpNotFound();
            }
            return View(alumno);
        }

        //
        // GET: /Alumno/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Alumno/Create

        [HttpPost]
        public ActionResult Create(AlumnoModelo alumno)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var modelo = new Alumno
                    {
                        AlumnoNombre = alumno.AlumnoNombre,
                        AlumnoApellido = alumno.AlumnoApellido
                    };

                    var run = (new RolUnicoVerificador(new RolUnicoNacional() { Rut = alumno.RolUnico }));

                    if (run.EsValido())
                    {
                        var numb = run.ObtenerRolUnico().Numero;
                        var alumn = db.Alumno.SingleOrDefault(o => o.AlumnoRut == numb);

                        if (alumn != null)
                        {
                            Danger("El alumno ya existe", true);
                        }
                        else
                        {
                            modelo.AlumnoRut = run.ObtenerRolUnico().Numero;
                            modelo.AlumnoRutDigito = run.ObtenerRolUnico().DigitoVerificador;

                            db.Alumno.AddObject(modelo);
                            db.SaveChanges();

                            return RedirectToAction("Index");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Danger(ex.Message, true);
            }

            return View(alumno);
        }

        //
        // GET: /Alumno/Edit/5

        public ActionResult Edit(int id = 0)
        {
            Alumno alumno = db.Alumno.Single(a => a.AlumnoId == id);
            if (alumno == null)
            {
                return HttpNotFound();
            }
            return View(alumno);
        }

        //
        // POST: /Alumno/Edit/5

        [HttpPost]
        public ActionResult Edit(Alumno alumno)
        {
            if (ModelState.IsValid)
            {
                db.Alumno.Attach(alumno);
                db.ObjectStateManager.ChangeObjectState(alumno, EntityState.Modified);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(alumno);
        }

        //
        // GET: /Alumno/Delete/5

        public ActionResult Delete(int id = 0)
        {
            Alumno alumno = db.Alumno.Single(a => a.AlumnoId == id);
            if (alumno == null)
            {
                return HttpNotFound();
            }
            return View(alumno);
        }

        //
        // POST: /Alumno/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                Alumno alumno = db.Alumno.Single(a => a.AlumnoId == id);
                db.Alumno.DeleteObject(alumno);
                db.SaveChanges();
            }
            catch (System.Data.UpdateException e)
            {
                var ex = e.GetBaseException() as SqlException;

                if (ex != null)
                {
                    if (ex.Errors.Count > 0)
                    {
                        switch (ex.Errors[0].Number)
                        {
                            case 547:
                                Danger("No puede eliminar esta alumno porque está siendo usada por otra entidad", true);
                                break;
                        }
                    }
                }
            }

            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}