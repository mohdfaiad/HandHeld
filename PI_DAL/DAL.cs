using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PI_Models;
using System.Web.Mvc;
using System.Data.Entity.Validation;

namespace PI_DAL
{
    public class DAL
    {

        PI_Domain.AlinIQ_DataEntities entities = new PI_Domain.AlinIQ_DataEntities();
        public Dictionary<string, string> SerialNumbers;
        public Dictionary<string, long> EPCSerialNumber;



        public IEnumerable<PhysicalItem> GetPhysicalItems()
        {

            List<PhysicalItem> ret = new List<PhysicalItem>();
            var physicalitems = entities.IMS_PHYSICAL_ITEMS;

            foreach (var item in physicalitems)
            {
                ret.Add(new PhysicalItem()
                {
                    Discrepancy = item.DISCREPANCY,
                    DiscrepancyNewLocation = item.DISCREPANCYNEWLOCATION,
                    DispositionParm = item.DISPOSITION_PARM,
                    DispositionStatus = item.DISPOSITION_STATUS,
                    NewLocationCode = item.NEWLOCATIONCODE,
                    Overridden = item.OVERRIDDEN,
                    PhysicalItemID = item.PHYSICALITEMID,
                    Processed = item.PROCESSED,
                    ScanItemStatus = item.SCAN_ITEM_STATUS,
                    SGTIN = item.SGTIN,
                    Status = item.STATUS
                });

            }

            return ret;
        }


        public IEnumerable<PhysicalInventory> GetPhysicalInventories()
        {

            List<PhysicalInventory> ret = new List<PhysicalInventory>();

            var physicalInventories = entities.IMS_PHYSICAL_INVENTORY;

            foreach (var i in physicalInventories)
            {
                ret.Add(new PhysicalInventory()
                {
                    EndDate = i.END_DATE,
                    LocationCode = i.LOCATIONCODE,
                    LocationName = i.LOCATION_NAME,
                    LocationShortName = i.LOCATION_SHORTNAME,
                    PhysicalInventoryID = i.PHYSICALINVENTORYID,
                    PhysicalInventoryReviewID = i.PHYSICALINVENTORYREVIEWID,
                    Status = i.STATUS
                });

            }

            return ret;
        }




        #region Mobile"
        public BooleanPlus StartPhysicalInventory(string locationCode)
        {
            var LocationInfo = from loc in entities.LOCATIONS where loc.LOCATIONCODE == locationCode select new { LOCATION_NAME = (loc.FULLNAME == "" ? loc.LOCATION_NAME : loc.FULLNAME), LOCATION_SHORTNAME = loc.LOCATION_NAME };

            var ret = CreatePhysicalInventory("LAYRJ", LocationInfo.FirstOrDefault().LOCATION_NAME, locationCode, LocationInfo.FirstOrDefault().LOCATION_SHORTNAME);
            UpdateCounter(ret.ID, "IMS_PHYSICAL_INVENTORY", "PHYSICALINVENTORYID");
            var bp = CreateCurrentInStockRecordSetForPhysicalInventory(locationCode, ret.ID);

            return ret;

        }


        private BooleanPlus CreatePhysicalInventory(string username, string locationName, string locationCode, string shortname)
        {

            BooleanPlus bp = new BooleanPlus();

            DateTime startDate = DateTime.Now;
            string status = "In Process";

            bp.ID = entities.IMS_PHYSICAL_INVENTORY.Max(u => (int?)u.PHYSICALINVENTORYID).GetValueOrDefault() + 1;

            entities.IMS_PHYSICAL_INVENTORY.Add(new PI_Domain.IMS_PHYSICAL_INVENTORY() { PHYSICALINVENTORYID = bp.ID, START_DATE = startDate, STATUS = status, USRNAM = username, LOCATION_NAME = locationName, LOCATIONCODE = locationCode, LOCATION_SHORTNAME = shortname, ORIGSTS = "N" });

            try
            {

                entities.SaveChanges();
            }
            catch (DbEntityValidationException ex)
            {
                var errorMessages = ex.EntityValidationErrors
                   .SelectMany(x => x.ValidationErrors)
                   .Select(x => x.ErrorMessage);

                var fullErrorMessage = string.Join("; ", errorMessages);
                var exceptionMessage = string.Concat(ex.Message, " The validation errors are: ", fullErrorMessage);

                bp.Errors = exceptionMessage;

            }

            return bp;

        }

        private BooleanPlus CreateCurrentInStockRecordSetForPhysicalInventory(string locationCode, int physicalInventoryID)
        {


            //nItemId := ExecFunction("InventoryManagementSolution.GetSequenceId", {"IMS_PHYSICAL_ITEMS", "PHYSICALITEMID"});
            //SqlExecute("INSERT INTO IMS_PHYSICAL_ITEMS (PHYSICALITEMID, SGTIN, PRODUCT_NAME, SUPPLIER_NAME, STATUS, STATUS_TIME, LOTNUMBER, LOTEXPIRATION, PHYSICALINVENTORYID, TAG_GTIN14, TAG_SN) 
            //            VALUES (?nItemId?, ?aItems[i][1]?, ?aItems[i][2]?, ?aItems[i][3]?, 'Expected', ?Now()?, ?aItems[i][4]?, ?aItems[i][5]?, ?nSessionId?, ?aItems[i][6]?, ?aItems[i][7]?)");
            BooleanPlus ret = new BooleanPlus();


            // int nextID = entities.IMS_PHYSICAL_ITEMS.Max(u => u.PHYSICALITEMID);

            IEnumerable<InventoryItem> StockItems = from stock in GetInventoryItemsByLocation(locationCode) where (stock.intransit == null || stock.intransit == "N") && (stock.status == "Pending" || stock.status == "Stock") orderby stock.itemID select stock;

            ret.count = StockItems.Count();

            string status = "Expected";

            foreach (var i in StockItems)
            {

                AddPhysicalInventoryItem(physicalInventoryID, status, i);

                try
                {
                    entities.SaveChanges();
                }
                catch (DbEntityValidationException ex)
                {
                    var errorMessages = ex.EntityValidationErrors
                       .SelectMany(x => x.ValidationErrors)
                       .Select(x => x.ErrorMessage);

                    var fullErrorMessage = string.Join("; ", errorMessages);
                    var exceptionMessage = string.Concat(ex.Message, " The validation errors are: ", fullErrorMessage);

                    ret.Errors = exceptionMessage;

                }

            }



            return ret;

        }

        private void AddPhysicalInventoryItem(int physicalInventoryID, string status, InventoryItem i)
        {

            int nextID = entities.IMS_PHYSICAL_ITEMS.Max(u => (int?) u.PHYSICALITEMID).GetValueOrDefault();

            nextID += 1;

            //UpdateCounter(nextID, "IMS_PHYSICAL_ITEMS", "PHYSICALITEMID");

            entities.IMS_PHYSICAL_ITEMS.Add(new PI_Domain.IMS_PHYSICAL_ITEMS()
            {
                ORIGSTS = "N",
                PHYSICALITEMID = nextID,
                SGTIN = i.sgtin,
                PRODUCT_NAME = i.productname,
                SUPPLIER_NAME = i.suppname,
                STATUS = status,
                STATUS_TIME = DateTime.Now,
                LOTNUMBER = i.lotnumber,
                LOTEXPIRATION = i.lotexpiration,
                PHYSICALINVENTORYID = physicalInventoryID,
                TAG_GTIN14 = i.taggtin14,
                TAG_SN = i.tagsn
            });
        }


        public void CompletePhysicalInventory(int sessionID, string locationCode, IEnumerable<Found> found)
        {


            var foundloc = found.Where(l => l.LocationCode == locationCode).ToList();


            foreach (var x in foundloc)
            {

                var update = entities.IMS_PHYSICAL_ITEMS.Where(z => z.PHYSICALINVENTORYID == sessionID && z.SGTIN == x.SGTIN).SingleOrDefault();


                if (update != null)
                {
                    update.STATUS = "Found";

                }
                else
                {
                    AddUnexpected(sessionID, x);
                }
                entities.SaveChanges();

            }


            MarkComplete(sessionID);

            //Mark Session as Complete
        }




        public int UpdateCounterPI()
        {
            var counter = entities.LIMSCOUNTERS.Where(x => x.TABLNAME == "IMS_PHYSICAL_ITEMS" && x.FLDNAME == "PHYSICALITEMID").SingleOrDefault();
            int maxID = entities.IMS_PHYSICAL_ITEMS.Max(x => (int?)x.PHYSICALITEMID).GetValueOrDefault(); 

            if (counter != null)
            {
                counter.LIMSCOUNTER1 = maxID;
                entities.SaveChanges();
            }

            return maxID;
        }

        private void AddUnexpected(int sessionID, Found sgtin)
        {

            InventoryItem unexpectedItem = null;

            var imsitem = (from i in entities.IMS_ITEMS where i.SGTIN == sgtin.SGTIN select i).SingleOrDefault();


            if (imsitem != null)
            {
                unexpectedItem = (from item in GetInventoryItems() where item.sgtin == sgtin.SGTIN select item).SingleOrDefault();
                AddPhysicalInventoryItem(sessionID, "Unexpected", unexpectedItem);
            }
            else
            {

                unexpectedItem = new InventoryItem();
                unexpectedItem.sgtin = sgtin.SGTIN;
                unexpectedItem.productname = sgtin.unkownItem.IsValid ? sgtin.unkownItem.productname : "NA";
                unexpectedItem.tagsn = sgtin.unkownItem.IsValid ? sgtin.unkownItem.serialnumber : "NA";
                unexpectedItem.taggtin14 = sgtin.unkownItem.IsValid ? sgtin.unkownItem.gtin : "NA";
                unexpectedItem.suppname = "NA";

                AddPhysicalInventoryItem(sessionID, "Unexpected", unexpectedItem);
            }

        }


        private void MarkComplete(int sessionID)
        {
            var pi = entities.IMS_PHYSICAL_INVENTORY.Where(x => x.PHYSICALINVENTORYID == sessionID).SingleOrDefault();
            pi.STATUS = "Complete";
            pi.USRNAM = "LAYRJ";
            pi.END_DATE = DateTime.Now;
            entities.SaveChanges();

        }


        private void UpdateCounter(int counter, string tablename, string fldname)
        {

            var ctr = entities.LIMSCOUNTERS.Where(x => x.TABLNAME == tablename && x.FLDNAME == fldname).SingleOrDefault();
            ctr.LIMSCOUNTER1 = counter + 1;
            entities.SaveChanges();

        }

        private int GetNextID(string tablename, string fldname)
        {
            var ret = entities.LIMSCOUNTERS.Where(x => x.TABLNAME == tablename && x.FLDNAME == fldname).SingleOrDefault();
            return ret.LIMSCOUNTER1;

        }


        public IEnumerable<Found> GetFoundItems(Handheld hh)
        {
            List<Found> ret = new List<Found>();

            foreach (var x in hh.locations)
            {
                if ((x.Select || x.MoveItemsOnly) && x.items != null)
                {
                    foreach (var z in x.items)
                    {
                        if (z.found)
                        {
                            ret.Add(new Found() { SGTIN = z.sgtin, LocationCode = z.changeLocation != null ? z.changeLocation : x.locationCode, LastLocation = x.locationCode });
                        }
                    }
                }
            }

            //GetDummySGTINS(hh, ref ret);

            return ret;
        }



        private void GetDummySGTINS(Handheld hh, ref List<Found> found)
        {
            foreach (var p in hh.products)
            {
                if (p.qty > 0)
                {

                    var SGTINS = CreateFakeSGTINs(p.ProductID, p.qty);

                    foreach (var x in SGTINS)
                    {
                        found.Add(new Found() { SGTIN = x.sgtin, LocationCode = p.location, unkownItem = x });
                    }
                }

            }

        }


        public IEnumerable<InventoryItem> GetInventoryItemsByLocation(string locationCode)
        {

            var ret = from I in entities.IMS_ITEMS
                      join LT in entities.IMS_LOTS on I.ITEMLOTID equals LT.LOTID into group1
                      from g1 in group1.DefaultIfEmpty()
                      join LC in entities.LOCATIONS on I.LOCATIONCODE equals LC.LOCATIONCODE into group2
                      from g2 in group2.DefaultIfEmpty()
                      join P in entities.IMS_PRODUCTS on I.PRODUCTID equals P.PRODUCTID into group3
                      from g3 in group3.DefaultIfEmpty()
                      join S in entities.SUPPLIERS on g3.SUPPCODE equals S.SUPPCODE into group4
                      from g4 in group4.DefaultIfEmpty()
                      where I.LOCATIONCODE == locationCode
                      select new InventoryItem()
                      {
                          itemID = I.ITEMID,
                          sgtin = I.SGTIN,
                          suppcode = g3.SUPPCODE,
                          suppname = g4.SUPPNAM,
                          lotnumber = g1.LOTNUMBER,
                          lotexpiration = g1.LOTEXPIRATION,
                          taggtin14 = I.TAG_GTIN14,
                          tagsn = I.TAG_SN,
                          shortname = g3.SHORT_NAME,
                          qty = g3.QUANTITY,
                          status = I.STATUS,
                          intransit = g2.INTRANSIT,
                          productname = g3.NAME
                      };


            return ret;
        }

        private IEnumerable<InventoryItem> GetInventoryItems()
        {
            var ret = from I in entities.IMS_ITEMS
                      join LT in entities.IMS_LOTS on I.ITEMLOTID equals LT.LOTID into group1
                      from g1 in group1.DefaultIfEmpty()
                      join LC in entities.LOCATIONS on I.LOCATIONCODE equals LC.LOCATIONCODE into group2
                      from g2 in group2.DefaultIfEmpty()
                      join P in entities.IMS_PRODUCTS on I.PRODUCTID equals P.PRODUCTID into group3
                      from g3 in group3.DefaultIfEmpty()
                      join S in entities.SUPPLIERS on g3.SUPPCODE equals S.SUPPCODE into group4
                      from g4 in group4.DefaultIfEmpty()
                      select new InventoryItem()
                      {
                          itemID = I.ITEMID,
                          sgtin = I.SGTIN,
                          suppcode = g3.SUPPCODE,
                          suppname = g4.SUPPNAM,
                          lotnumber = g1.LOTNUMBER,
                          lotexpiration = g1.LOTEXPIRATION,
                          taggtin14 = I.TAG_GTIN14,
                          tagsn = I.TAG_SN,
                          shortname = g3.SHORT_NAME,
                          qty = g3.QUANTITY,
                          status = I.STATUS,
                          intransit = g2.INTRANSIT,
                          productname = g3.NAME
                      };


            return ret;


        }


        private List<UnknownItem> CreateFakeSGTINs(int ProductID, int qty)
        {
            List<UnknownItem> ret = new List<UnknownItem>();
            string sgtin = "";

            var prod = (from i in entities.IMS_PRODUCTS where i.PRODUCTID == ProductID select i).SingleOrDefault();
            string sn = "";
            string epc = "urn:epc:id:sgtin:{0}.{1}";

            Random random = new Random();
            long randnum2;

            for (int i = 0; i < qty; i++)
            {
                randnum2 = (long)(random.NextDouble() * 9000000000) + 1000000000;
                sn = randnum2.ToString();
                sgtin = string.Format(epc, prod.GTIN, sn);
                ret.Add(new UnknownItem() { gtin = prod.GTIN, IsValid = true, productname = prod.NAME, serialnumber = sn, sgtin = sgtin });
            }

            return ret;
        }





        #endregion



        #region "Item Creation"

        public ItemCreation GetItemCreation(int qty)
        {
            ItemCreation ic = new ItemCreation();
            ic.products = (List<Product>)GetProductsForFakeSGTINs(qty);


            ic.products.ForEach(z =>
                {
                    z.AllocatedCostCenters = (from cc in GetAllocations(z.ProductID) select new SelectListItem() { Value = cc.CostCenterID.ToString(), Text = cc.CostCenterFullName }).ToList();

                });


            ic.locationsList = (from l in GetLocations() select new SelectListItem() { Value = l.locationCode, Text = l.locationCode + " (" + l.Site + ")" }).ToList();
            return ic;
        }


        private IEnumerable<CostCenter> GetAllocations(int productID)
        {
            var ret = from ccp in entities.IMS_COST_CENTER_PRODUCTS
                      join cc in entities.IMS_COST_CENTERS on ccp.COSTCENTERID equals cc.COST_CENTER_ID
                      where ccp.PRODUCTID == productID
                      select new CostCenter() { CostCenterID = cc.COST_CENTER_ID, Site = cc.DEPT, CostCenterFullName = cc.COST_CENTER_FULLNAME, CostCenterName = cc.COST_CENTER_NAME };
            return ret;

        }



        public BooleanPlus CreateItems(ItemCreation model)
        {
            BooleanPlus bp = new BooleanPlus();
            string sn = "";
            string epc = "urn:epc:id:sgtin:{0}.{1}";
            string sgtin = "";
            Int32 ItemID = 0;

            Random lotExention = new Random();
            Random random = new Random();
            long randnum2;


            model.products.Where(z => z.qty > 0 & z.location != null & z.CostCenterID != "0").ToList().ForEach(z =>
                {

                    var verifiedlot = z.lotnumber == null ? "test_" + lotExention.NextDouble().ToString() : z.lotnumber;

                    var lotID = CreateLot(verifiedlot, z.lotExpiry, z.ProductID);

                    for (int i = 0; i < z.qty; i++)
                    {

                        randnum2 = (long)(random.NextDouble() * 9000000000) + 1000000000;
                        sn = randnum2.ToString();
                        sgtin = string.Format(epc, z.GTIN, sn);




                        ItemID = GetNextID("IMS_ITEMS", "ITEMID") + 1;
                        entities.IMS_ITEMS.Add(new PI_Domain.IMS_ITEMS()
                        {
                            ORIGSTS = "N",
                            ITEMID = ItemID,
                            LOCATIONCODE = z.location,
                            LOCATIONTIME = DateTime.Now,
                            SGTIN = sgtin,
                            PRODUCTID = z.ProductID,
                            COSTCENTERID = Convert.ToInt32(z.CostCenterID),
                            STATUS = "Stock",
                            STATUSTIME = DateTime.Now,
                            ITEMLOTID = lotID,
                            TAG_GTIN14 = z.GTIN,
                            TAG_SN = sn,
                            CONSUMPTION_UNITS = 1,
                            SYSTEM_GENERATED_TAG = "N",
                            QUARANTINESTATUS = "Do not Quarantine"
                        });



                        try
                        {

                            entities.SaveChanges();
                        }
                        catch (DbEntityValidationException ex)
                        {
                            var errorMessages = ex.EntityValidationErrors
                               .SelectMany(x => x.ValidationErrors)
                               .Select(x => x.ErrorMessage);

                            var fullErrorMessage = string.Join("; ", errorMessages);
                            var exceptionMessage = string.Concat(ex.Message, " The validation errors are: ", fullErrorMessage);

                            bp.Errors = exceptionMessage;

                        }


                        UpdateCounter(ItemID, "IMS_ITEMS", "ITEMID");

                    }



                });



            return bp;
        }


        private int CreateLot(string lotnumber, DateTime expirationDate, int ProductID)
        {
            int ret = 0;

            var id = GetNextID("IMS_LOTS", "LOTID") + 1;

            var lotExists = LotNumberExists(lotnumber);

            var lot = new PI_Domain.IMS_LOTS() { LOTID = id, LOTEXPIRATION = expirationDate, LOTNUMBER = lotExists.value, ORIGSTS = "N", PRODUCTID = ProductID };

            entities.IMS_LOTS.Add(lot);

            try
            {

                UpdateCounter(id, "IMS_LOTS", "LOTID");
                entities.SaveChanges();
            }
            catch (Exception ex)
            {
                var z = ex;

            }

            ret = lot.LOTID;

            return ret;
        }


        private BooleanPlus LotNumberExists(string lotnumber)
        {
            BooleanPlus ret = new BooleanPlus();

            var Exists = (from l in entities.IMS_LOTS where l.LOTNUMBER == lotnumber select l).Any();


            if (Exists)
            {
                ret.status = false;

                if (lotnumber.Length < 50)
                {
                    for (int i = 0; i < 10; i++)
                    {
                        var x = LotNumberExists(lotnumber + i.ToString());
                        if (x.status)
                        {
                            ret.value = lotnumber + i.ToString();
                            ret.status = true;
                            return ret;
                        }
                    }
                }

            }
            else
            {
                ret.value = lotnumber;
                ret.status = true;
            }



            return ret;

        }



        #endregion


        #region test harness



        public Handheld GetHandHeld(int take = 1000)
        {
            Handheld hh = new Handheld();
            hh.locations = GetLocations(take: take);
            hh.locationsList = GeltSelectListForLocations();
            //hh.products = (List<Product>)GetProductsForFakeSGTINs();
            return hh;

        }


        public HandHeldLocation GetHandHeldForLocation(string locationCode)
        {
            HandHeldLocation hh = new HandHeldLocation();
            hh.location = GetLocations().Where(l => l.locationCode == locationCode).SingleOrDefault();
            hh.locationsList = GeltSelectListForLocations();

            return hh;

        }


        public IList<SelectListItem> GeltSelectListForLocations()
        {
            return (from loc in entities.LOCATIONS select new SelectListItem() { Value = loc.LOCATIONCODE, Text = loc.LOCATION_NAME + " (" + loc.LOCATIONCODE + ")" }).ToList();
        }


        public IList<Product> GetProductsForFakeSGTINs(int qty = 10)
        {
            return (from prod in entities.IMS_PRODUCTS select new Product() { GTIN = prod.GTIN, Name = prod.SHORT_NAME, ProductID = prod.PRODUCTID, qty = 0 }).Take(qty).ToList();
        }


        public List<Location> GetLocations(bool includeItems = true, int take = 1000)
        {
            List<Location> locations = (from loc in entities.LOCATIONS
                                        join r in entities.ROOMS on loc.ROOM_ID equals r.ROOM_ID
                                        join b in entities.BUILDINGS on r.BUILDING_ID equals b.BUILDING_ID
                                        join d in entities.DEPARTMENTS on b.DEPT equals d.DEPT
                                        select new Location() { locationCode = loc.LOCATIONCODE, locationName = loc.LOCATION_NAME, Site = d.DEPT }).ToList();
            if (includeItems)
            {
                GetLocationItems(locations, take);
            }




            locations.Add(GetShipped());


            return locations;

        }


        public Location GetShipped()
        {

            Location shipped = new Location();

            shipped.locationCode = "Shipped";
            shipped.locationName = "Items with shipped Status";
            shipped.Skip = true;
            shipped.items =
                  (from item in entities.IMS_ITEMS
                   where item.STATUS == "Shipped"
                   select new Item()
                   {
                       costcenterID = item.COSTCENTERID,
                       itemID = item.ITEMID,
                       sgtin = item.SGTIN,
                       status = item.STATUS,
                       statusTime = item.STATUSTIME
                   }).ToList();


            GetCostCenterName(shipped.items);

            //foreach(var z in shipped.items)
            //{

            //    if(z.costcenterID != null)
            //    {
            //        z.costcentername = (from cc in entities.IMS_COST_CENTERS where cc.COST_CENTER_ID == z.costcenterID select cc.COST_CENTER_NAME).SingleOrDefault();
            //    }
            //}

            return shipped;

        }



        private void GetLocationItems(List<Location> locations, int take)
        {
            foreach (var l in locations)
            {
                l.items = (from item in entities.IMS_ITEMS
                           join cc in entities.IMS_COST_CENTERS on item.COSTCENTERID
                           equals cc.COST_CENTER_ID into grp
                           from cc in grp.DefaultIfEmpty()
                           where item.LOCATIONCODE == l.locationCode
                           select new Item()
                               {
                                   costcenterID = item.COSTCENTERID,
                                   itemID = item.ITEMID,
                                   sgtin = item.SGTIN,
                                   status = item.STATUS,
                                   statusTime = item.STATUSTIME,
                                   unknownItem = false,
                                   costcentername = cc.COST_CENTER_NAME
                               }).Take(take).ToList();



                for (int i = 0; i < 4; i++)
                {
                    l.items.Add(new Item() { unknownItem = true, sgtin = "na" });
                }


            }

        }



        private void GetCostCenterName(List<Item> items)
        {
            foreach (var z in items)
            {

                if (z.costcenterID != null)
                {
                    z.costcentername = (from cc in entities.IMS_COST_CENTERS where cc.COST_CENTER_ID == z.costcenterID select cc.COST_CENTER_NAME).SingleOrDefault();
                }
            }


        }


        #endregion



        #region "Reviews"


        public IList<PI_Models.PhysicalInventory> GetScansFoReviw()
        {

            IList<PI_Models.PhysicalInventory> ret = (from rev in entities.IMS_PHYSICAL_INVENTORY
                                                      where rev.STATUS == "Review"

                                                      select new PhysicalInventory()
                                                      {
                                                          PhysicalInventoryID = rev.PHYSICALINVENTORYID,
                                                          LocationName = rev.LOCATION_NAME,
                                                          Select = false,
                                                          StartDate = rev.START_DATE,
                                                          User = rev.USRNAM,
                                                          Status = rev.STATUS
                                                      }).ToList();
            ret.OrderBy(d => d.StartDate);

            return ret;
        }




        #endregion




        #region "ASN"


        public AdvancedShipingNotice CreateASN()
        {
            AdvancedShipingNotice model = new AdvancedShipingNotice();
            model.PurchaseOrders = GetPurchaseOrders();
            model.ShipToCustomers = GetShipToCustomers();


            return model;

        }

        public List<PurchaseOrder> GetPurchaseOrders()
        {


            List<PurchaseOrder> PurchaseOrders = (from po in entities.IMS_PURCHASE_ORDERS
                                                  join cc in entities.IMS_COST_CENTERS on po.COST_CENTER_ID equals cc.COST_CENTER_ID
                                                  where po.STATUS == "Approved"
                                                  orderby po.PO_NUMBER descending
                                                  select new PurchaseOrder()
                                                  {
                                                      ApprovalDate = po.APPROVAL_DATE,
                                                      CostCenterID = po.COST_CENTER_ID,
                                                      CustomerID = po.CUSTOMERID,
                                                      POCustomerRef = po.PO_CUSTOMER_REF,
                                                      PONumber = po.PO_NUMBER,
                                                      PurchaseOrderID = po.PURCHASEORDERID,
                                                      Status = po.STATUS,
                                                      CostCenterName = cc.COST_CENTER_NAME,
                                                      Site = cc.DEPT,
                                                      asn = new ASN()
                                                  }).ToList();

            foreach (var x in PurchaseOrders)
            {
                x.LineItems = GetPurchaseOrderLineItems(x.PurchaseOrderID);
            }


            return PurchaseOrders.Where(x => x.LineItems.Any()).ToList();

        }

        public List<PurchaseOrderLineItem> GetPurchaseOrderLineItems(int PurchaseOrderID)
        {
            List<PurchaseOrderLineItem> lineItems = (from item in entities.IMS_PO_LINE_ITEMS
                                                     join prod in entities.IMS_PRODUCTS on item.PRODUCTID equals prod.PRODUCTID
                                                     where item.PURCHASEORDERID == PurchaseOrderID
                                                     select new PurchaseOrderLineItem()
                                                  {
                                                      ASNQty = item.QUANTITY_PENDING,
                                                      LineItemNumber = item.LINE_ITEM_NUMBER,
                                                      OrderLineItemID = item.ORDERLINEITEMID,
                                                      QuantityAccepted = item.QUANTITY_ACCEPTED,
                                                      QuantityOrdered = item.QUANTITY_ORDERED,
                                                      Select = false,
                                                      Status = item.STATUS,
                                                      ProductID = item.PRODUCTID,
                                                      GTIN = prod.GTIN,
                                                      ProductName = prod.NAME
                                                  }).ToList();



            GetProductCustomerID(ref lineItems);


            return lineItems;
        }



        public void GetProductCustomerID(ref List<PurchaseOrderLineItem> LineItems)
        {
            foreach (var z in LineItems)
            {

                var SupplierID = (from p in entities.IMS_PRODUCTS where p.PRODUCTID == z.ProductID join sup in entities.SUPPLIERS on p.SUPPCODE equals sup.SUPPCODE select sup.SUPPID).SingleOrDefault();

                z.SupplierID = SupplierID == null ? "NA" : SupplierID.ToString();


            }
        }


        public void CreateSerialNumberDictiony()
        {

            SerialNumbers = new Dictionary<string, string>();
            EPCSerialNumber = new Dictionary<string, long>();
        }


        public BooleanPlus ReceiveASN(PurchaseOrder purchaseOrder, AdvancedShipingNotice asn)
        {
            BooleanPlus bp = new BooleanPlus();

            if (asn.DirectInsert)
            {


            }

            if (asn.XmlFileOutput)
            {


                ASNxml xAsn = XmlOutput(purchaseOrder);

                XmlWriter w = new XmlWriter();
                w.WriteToFile(xAsn);
                XmlFormatter formatter = new XmlFormatter();
                var zz = formatter.Serialize<ASNxml>(xAsn);
                bp.value += zz;
            }

            return bp;

        }




        private ASNxml XmlOutput(PurchaseOrder po)
        {
            ASNxml ret = new ASNxml();
            ret.CreationDate = DateTime.Now.ToLongTimeString();
            ret.Number = po.asn.ASNNumber;
            ret.ShipmentDate = po.asn.ShipmentDate.ToString();
            ret.ShipToCustomerID = po.asn.ShipToCustomerID;
            string customerPrefix = "";

            List<PalletXml> pallets = new List<PalletXml>();
            List<CartonXml> cartons = new List<CartonXml>();
            List<LineItemXml> lineItems = new List<LineItemXml>();
            List<IndividualItemXml> IndividualItems = new List<IndividualItemXml>();
            var p = new PalletXml();
            var c = new CartonXml();

            int li = 0;
            foreach (var x in po.LineItems)
            {
                if (x.Select)
                {
                    var l = new LineItemXml();

                    var product = (from zz in entities.IMS_PRODUCTS where zz.GTIN == x.GTIN select zz).SingleOrDefault();
                    var customerSupplier = (from sc in entities.IMS_SUPPLIER_CUSTOMERS where sc.CUSTOMERID == po.CustomerID select sc).SingleOrDefault();
                    customerPrefix = product.COMPANY_PREFIX == null ? "000" : product.COMPANY_PREFIX;


                    if (product != null)
                    {

                        l.Component = "1";
                        l.PONumber = po.PONumber;
                        ret.SupplierID = x.SupplierID;
                        li++;
                        l.POLineItem = li.ToString();
                        l.CustomerReferenceNumber = po.POCustomerRef;
                        l.ProductID = product.LISTNUMBER; ;
                        l.GTIN = x.GTIN;
                        l.LotNumber = x.LotNumber == null ? Guid.NewGuid().ToString().Substring(20) : x.LotNumber;

                        l.ExpirationDate = x.LotExpiration.ToShortDateString();
                        l.QuantityOrdered = x.QuantityOrdered.ToString();
                        l.QuantityShipped = x.ASNQty.ToString();

                        IndividualItems = new List<IndividualItemXml>();
                        IndividualItemXml ii = new IndividualItemXml();

                        for (int i = 1; i < x.ASNQty + 1; i++)
                        {



                            ii = new IndividualItemXml();
                            ii.epc = string.Format("urn:epc:id:sgtin:{0}.{1}.{2}", product.COMPANY_PREFIX, x.GTIN.Substring(product.COMPANY_PREFIX.Length + 1), GetNextSerialNumber(product.GTIN));
                            IndividualItems.Add(ii);
                        }

                        l.individualitems = IndividualItems;
                        lineItems.Add(l);
                    }

                    c.lineitems = lineItems;
                }

            }

            c.epc = GetEpc(customerPrefix);
            cartons.Add(c);
            p.cartons = cartons;
            p.epc = GetEpc(customerPrefix);
            pallets.Add(p);
            ret.pallets = pallets;

            return ret;
        }


        private string GetNextSerialNumber(string gtin)
        {

            string ret = "";
            long zz = 0;


            if (SerialNumbers.ContainsKey(gtin))
            {
                ret = SerialNumbers[gtin];


                if (long.TryParse(ret, out zz))
                {
                    zz = zz + 1;
                    SerialNumbers[gtin] = zz.ToString();
                    ret = zz.ToString();
                    ret = MakeLength(ret, 7);
                }
            }
            else
            {
                var zzz = (from it in entities.IMS_ITEMS
                           join p in entities.IMS_PRODUCTS on it.PRODUCTID equals p.PRODUCTID
                           where p.GTIN == gtin
                           select it.TAG_SN).ToList().Count;

                if (zzz > 0)
                {
                    var list = (from it in entities.IMS_ITEMS
                                join p in entities.IMS_PRODUCTS on it.PRODUCTID equals p.PRODUCTID
                                where p.GTIN == gtin
                                select it.TAG_SN).ToList();
                    zz = list.Select(long.Parse).ToList().Max();
                }
                else
                {
                    zz = 1;
                }

                zz += 1;
                SerialNumbers.Add(gtin, zz.ToString());
                ret = MakeLength(zz.ToString(), 7);
            }

            return ret;
        }

        private static string MakeLength(string ret, int len)
        {
            int c = ret.Length;

            for (; c < len; c++)
            {
                ret = "0" + ret;
            }
            return ret;
        }


        private BooleanPlus Direct(PurchaseOrder purchaseOrder)
        {
            BooleanPlus ret = new BooleanPlus();

            CreateShipment(purchaseOrder.CustomerID, purchaseOrder.asn.ASNNumber, (DateTime)purchaseOrder.asn.ShipmentDate);

            foreach (var x in purchaseOrder.LineItems)
            {

            }


            return ret;
        }

        private BooleanPlus CreateShipment(int CustomerID, string ASNNumber, DateTime ShipmentDate, string ReceiveStatus = "Expected")
        {

            BooleanPlus bp = new BooleanPlus();

            string tablename = "IMS_SHIPMENTS";
            string fldname = "SHIPMENTID";

            int nextID = entities.IMS_SHIPMENTS.Max(x => x.SHIPMENTID);

            nextID += 1;

            entities.IMS_SHIPMENTS.Add(new PI_Domain.IMS_SHIPMENTS
            {
                ORIGSTS = "N",
                CUSTOMERID = CustomerID,
                ASNCREATIONDATE = DateTime.Now,
                ASNNUMBER = ASNNumber,
                RECEIVESTATUS = ReceiveStatus,
                HAS_SYSTEM_GENERATED_TAGS = "N",
                SHIPMENTDATE = ShipmentDate,
                SHIPMENTID = nextID,
                STATUSTIME = DateTime.Now
            });

            UpdateCounter(nextID, tablename, fldname);

            entities.SaveChanges();

            bp.ID = nextID;

            return bp;
        }


        public List<Supplier> GetSuppliers()
        {

            var ret = (from s in entities.SUPPLIERS
                       where s.SUPPID != null && s.SUPPID != "0" && s.SUPPID != ""
                       select new Supplier()
                       {
                           SuppCode = s.SUPPCODE,
                           SuppID = s.SUPPID,
                           SuppName = s.SUPPNAM
                       }).OrderBy(x => x.SuppID).ToList();


            return ret;
        }


        public string GetEpc(string companyPrefix)
        {

            string ret;
            TagParts parts = new TagParts();
            List<TagParts> tags = new List<TagParts>();
            TagParts tp;
            int half;

            //get all shipment container epc codes
            var sscc = entities.IMS_SHIPMENT_CONTAINERS.Where(x => x.SSCC.StartsWith("urn:epc:id:sscc")).ToList();

            foreach (var t in sscc)
            {

                tp = new TagParts();
                half = t.SSCC.LastIndexOf(':');
                tp.prefix = t.SSCC.Substring(0, half);
                string zz = t.SSCC.Substring(half + 1, t.SSCC.Length - (half + 1));
                var arry = zz.Split('.');

                //parse tag for serial number

                tp.companyPrefix = arry[0];
                tp.partnumber = arry[1];

                long parseme = 0;


                if (arry.Length > 2)
                {
                    if (long.TryParse(arry[2], out parseme))
                    {
                        tp.serialNumber = arry[2];

                        if (EPCSerialNumber.ContainsKey(tp.companyPrefix))
                        {
                            if (EPCSerialNumber[tp.companyPrefix] < parseme)
                            {
                                EPCSerialNumber[tp.companyPrefix] = parseme;
                            }
                        }
                        else
                        {
                            EPCSerialNumber.Add(tp.companyPrefix, parseme);
                        }
                    }
                }
                tags.Add(tp);
            }

            parts.serialNumber = GetNewEPCSerialNumber(companyPrefix);

            //Get get next new numbers from tags

            long test = 0;
            long partnumber = 0;

            if (tags.Where(v => v.companyPrefix == companyPrefix).ToList().Count > 0)
            {
                foreach (var sn in tags.Where(v => v.companyPrefix == companyPrefix).ToList())
                {
                    test = 0;

                    if (long.TryParse(sn.partnumber, out test))
                    {
                        if (test > partnumber)
                        {
                            partnumber = test;
                        }
                    }
                }

                parts.partnumber = partnumber.ToString();
            }
            else
            {
                parts.partnumber = "1000001";
            }


            parts.companyPrefix = companyPrefix;



            ret = string.Format("urn:epc:id:sscc:{0}.{1}.{2}", companyPrefix, parts.partnumber, parts.serialNumber);

            return ret;

        }

        public List<SelectListItem> GetShipToCustomers()
        {

            var ret = (from s in entities.IMS_SUPPLIER_CUSTOMERS
                       select new SelectListItem()
                         {
                             Text = s.NAME + (s.CITY == null ? "" : " " + s.CITY),
                             Value = s.SHIP_TO_CUSTOMER_ID
                         }).OrderBy(x => x.Text).ToList();


            return ret;
        }


        private string GetNewEPCSerialNumber(string customerPrefix)
        {
            long ret = 0;

            if (EPCSerialNumber.ContainsKey(customerPrefix))
            {
                ret = EPCSerialNumber[customerPrefix] + 1;
                EPCSerialNumber[customerPrefix] = ret;
            }
            else
            {
                ret = 1000001;
                EPCSerialNumber.Add(customerPrefix, ret);
            }

            return ret.ToString();
        }





        #endregion
    }
}
