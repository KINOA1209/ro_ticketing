using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ticketing_api.Models;
using ticketing_api.Models.Views;

namespace ticketing_api.Controllers
{
    partial class BillOfLadingsController : BaseController.BaseController
    {
        /// <summary>
        /// Get list of products for billoflading
        /// </summary>
        /// <param name="id">billoflading Id</param> 
        /// <returns></returns>
        [HttpGet("{id}/billofladingproducts")]
        public async Task<IActionResult> GetBillOfLadingProductsAsync(int id)
        {
            var billoflading = await _context.BillOfLading.FindAsync(id);
            var errorMsg = await _billOfLadingService.CheckBillOfLadingAccessAsync(billoflading, AppUser);
            if (errorMsg != null) return BadRequest(errorMsg);

            var products = _billOfLadingService.GetBillOfLadingProducts(id);

            var billofladingBillOfLadingProductView = new BillOfLadingBillOfLadingProductView() { BillOfLading = billoflading, BillOfLadingProducts = products };
            return Ok(billofladingBillOfLadingProductView);
        }

        /// <summary>
        /// Add products to billoflading
        /// </summary>
        /// <param name="id">billofladingId</param>
        /// <param name="billofladingProducts"></param>
        /// <returns></returns>
        [HttpPost("{id}/billofladingproducts")]
        public async Task<IActionResult> PostBillOfLadingProductAsync([FromRoute] int id, [FromBody] List<BillOfLadingProduct> billofladingProducts)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var billoflading = await _context.BillOfLading.FindAsync(id);
            var errorMsg = await _billOfLadingService.CheckBillOfLadingAccessAsync(billoflading, AppUser);
            if (errorMsg != null) return BadRequest(errorMsg);

            foreach (var billofladingProduct in billofladingProducts)
            {
                if (billofladingProduct.BillOfLadingId != id)
                    return BadRequest("BillOfLadingProduct is not for this billoflading id");
            }

            var resultViews = new List<BillOfLadingProductView>();
            foreach (var billofladingProduct in billofladingProducts)
            {
                _context.BillOfLadingProduct.Add(billofladingProduct);
                await _context.SaveChangesAsync();
                var resultView = _billOfLadingService.PostBillOfLadingProduct(billofladingProduct);
                resultViews.Add(resultView);
                //var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
                //await _billOfLadingService.StoreBillOfLadingProductLogInformation(id, resultView, this.ControllerContext.RouteData.Values["action"].ToString(), userId);
            }
            return Ok(resultViews);
        }

        /// <summary>
        /// Update an array of products for a billoflading
        /// </summary>
        /// <param name="id"></param>
        /// <param name="billofladingProducts"> list of the products to update</param>
        /// <returns></returns>
        //[Authorize(Policy = "AdminOnly")]
        [HttpPut("{id}/billofladingproducts")]
        public async Task<IActionResult> PutBillOfLadingProductsAsync([FromRoute] int id, [FromBody] List<BillOfLadingProduct> billofladingProducts)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            foreach (var billofladingProduct in billofladingProducts)
            {
                if (billofladingProduct.BillOfLadingId != id)
                    return BadRequest("BillOfLadingProduct is not for this billoflading id");
            }

            var billoflading = await _context.BillOfLading.FindAsync(id);
            var errorMsg = await _billOfLadingService.CheckBillOfLadingAccessAsync(billoflading, AppUser);
            if (errorMsg != null) return BadRequest(errorMsg);

            //To check the status of Voided and Delivered
            if (billoflading.BolStatusId == 4 || billoflading.BolStatusId == 5)
                return BadRequest("Cannot update BillOfLadingProduct when billoflading status is either Delivered or Voided");
            List<BillOfLadingProductView> billofladingProductUpdateAfterViewList = new List<BillOfLadingProductView>();
            foreach (var billofladingProduct in billofladingProducts)
            {
                var dbBillOfLadingProduct = _context.BillOfLadingProduct.AsNoTracking().FirstOrDefault(x => x.Id == billofladingProduct.Id);
                if (dbBillOfLadingProduct?.BillOfLadingId != id)
                    return BadRequest("BillOfLadingProduct is not for this billoflading id");
            }

            foreach (var billofladingProduct in billofladingProducts)
            {
                BillOfLadingProduct billofladingProductUpdateBefore =
                    _context.BillOfLadingProduct.AsNoTracking().FirstOrDefault(x => x.Id == billofladingProduct.Id);
                BillOfLadingProductView billofladingProductUpdateBeforeView =
                    _billOfLadingService.PostBillOfLadingProduct(billofladingProductUpdateBefore);

                // check that only quantity changed if role is DRIVER

                var role = _context.AppUser.FirstOrDefault(x => x.Role == User.FindFirst(ClaimTypes.Role).Value)?.Role.ToUpper();

                if (role == "DRIVER")
                {
                    var tProduct = _context.BillOfLadingProduct.FirstOrDefault(x => x.Id == billofladingProduct.Id);
                    tProduct.Quantity = billofladingProduct.Quantity;
                    _context.Entry(tProduct).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                }
                else
                {
                    _context.Entry(billofladingProduct).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                }


                var billofladingProductUpdateAfterView = _billOfLadingService.PostBillOfLadingProduct(billofladingProduct);
                billofladingProductUpdateAfterViewList.Add(_billOfLadingService.PostBillOfLadingProduct(billofladingProduct));
                //save product update change to log
                //var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
                //await _billOfLadingService.StoreBillOfLadingProductLogInformationPut(billofladingProductUpdateAfterView, billofladingProduct,
                //    this.ControllerContext.RouteData.Values["action"].ToString(), userId, billofladingProductUpdateBeforeView,
                //    billofladingProductUpdateBefore);
            }

            return Ok(billofladingProductUpdateAfterViewList);
        }

        /// <summary>
        /// Delete  billoflading product from billoflading
        /// </summary>
        /// <param name="id"></param>
        /// <param name="billofladingProductId"></param>
        /// <returns></returns>
        [HttpDelete("{id}/billofladingproducts/{billofladingProductId}")]
        public async Task<IActionResult> DeleteBillOfLadingProductAsync([FromRoute] int id, [FromRoute] int billofladingProductId)
        {
            var billoflading = await _context.BillOfLading.FindAsync(id);
            var errorMsg = await _billOfLadingService.CheckBillOfLadingAccessAsync(billoflading, AppUser);
            if (errorMsg != null) return BadRequest(errorMsg);

            var dbBillOfLadingProduct = await _context.BillOfLadingProduct.FindAsync(billofladingProductId);
            if (dbBillOfLadingProduct == null)
                return BadRequest("BillOfLadingProduct not found");
            if (dbBillOfLadingProduct.BillOfLadingId != id)
                return BadRequest("BillOfLadingProduct is not for this billoflading id");

            _context.BillOfLadingProduct.Remove(dbBillOfLadingProduct);
            await _context.SaveChangesAsync();

            //var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            //await _billOfLadingService.StoreLogInformationDelete(id, this.ControllerContext.RouteData.Values["action"].ToString(), userId);

            return Ok(dbBillOfLadingProduct);
        }
    }
}