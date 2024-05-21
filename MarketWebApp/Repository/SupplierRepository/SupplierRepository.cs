using iText.Commons.Actions.Contexts;
using MarketWebApp.Data;
using MarketWebApp.Models.Entity;
using MarketWebApp.ViewModel;
using MarketWebApp.ViewModel.Supplier;
using Microsoft.EntityFrameworkCore;
using System.Drawing.Drawing2D;

namespace MarketWebApp.Repository.SupplierRepository
{
    public class SupplierRepository : ISupplierRepository
    {
        private readonly ApplicationDbContext context;

        public SupplierRepository(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            this.context = context;
        }

        public IEnumerable<Supplier> SearchByName(string name)
        {
            var suppliers = context.Suppliers
                .Where(s => s.Name.Contains(name))
                .ToList();

            if (suppliers == null || !suppliers.Any())
            {
                return Enumerable.Empty<Supplier>(); 
            }

            return suppliers;
        }

        public bool CheckSupplierExist(string Name)
        {
            return context.Suppliers.SingleOrDefault(b => b.Name.ToLower() == Name.ToLower()) == null;
        }

        public bool CheckSupplierExistEdit(string Name, int Id)
        {
            return context.Suppliers.SingleOrDefault(b => b.ID != Id && b.Name.ToLower() == Name.ToLower()) == null;
        }

        public void Delete(int id)
        {
            var supplier = GetSupplier(id);

            if (supplier == null)
            {
                // Handle the case where the supplier with the given id is not found
                throw new InvalidOperationException("Supplier not found.");
            }

            context.Suppliers.Remove(supplier);
            Save();
        }

        public IEnumerable<Supplier> GetAll()
        {
            return context.Suppliers.ToList();
        }

        public void Update(EditSupplierViewModel editSupplierViewModel)
        {
            var supplier = GetSupplier(editSupplierViewModel.ID);

            if (supplier == null)
            {
                throw new ArgumentException("Supplier not found.");
            }

            if (string.IsNullOrWhiteSpace(editSupplierViewModel.Name))
            {
                throw new ArgumentException("Supplier name cannot be empty.");
            }

            supplier.Name = editSupplierViewModel.Name;
            supplier.Phone = editSupplierViewModel.Phone;
            supplier.Location = editSupplierViewModel.Location;

            Save(); // Save changes to the database
        }

        public Supplier GetSupplier(int Id)
        {
            return context.Suppliers.SingleOrDefault(b => b.ID == Id);
        }

        public Supplier GetSupplierWithProducts(int Id)
        {
            return context.Suppliers.Where(b => b.ID == Id).Include(b => b.Products).SingleOrDefault();
        }

        public void Insert(AddSupplierViewModel addSupplierViewModel)
        {
            var supplier = new Supplier();
            supplier.Name = addSupplierViewModel.Name;
            supplier.Phone = addSupplierViewModel.Phone;
            supplier.Location = addSupplierViewModel.Location;

            context.Suppliers.Add(supplier);
            Save();
        }

        public void Save()
        {
            context.SaveChanges();
        }

    }
}
