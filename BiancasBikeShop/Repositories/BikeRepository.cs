using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using BiancasBikeShop.Models;

namespace BiancasBikeShop.Repositories
{
    public class BikeRepository : IBikeRepository
    {
        private SqlConnection Connection
        {
            get
            {
                return new SqlConnection("server=localhost\\SQLExpress;database=BiancasBikeShop;integrated security=true;TrustServerCertificate=true");
            }
        }

        public List<Bike> GetAllBikes()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT b.Id, b.Brand, b.Color, o.Id OwnerId, o.[Name]
                        FROM dbo.Bike AS b
                        LEFT JOIN dbo.Owner AS o ON o.Id = b.OwnerId
                    ";

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        var bikes = new List<Bike>();

                        while (reader.Read())
                        {
                            Bike newBike = new()
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                Brand = reader.GetString(reader.GetOrdinal("Brand")),
                                Color = reader.GetString(reader.GetOrdinal("Color")),
                                Owner = new Owner()
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("OwnerId")),
                                    Name = reader.GetString(reader.GetOrdinal("Name")),
                                }
                            };

                            bikes.Add(newBike);
                        }

                        return bikes;
                    }
                }
            }
        }

        public Bike GetBikeById(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand()) 
                {
                    cmd.CommandText = @"
                        SELECT b.Id BikeId, b.Brand, b.Color, 
                               o.Id OwnerId, o.[Name] OwnerName, o.Address Address,
                               bt.Id BikeTypeId, bt.[Name] BikeTypeName,
                               wo.Id WorkOrderId, wo.DateInitiated, wo.Description, wo.DateCompleted
                        FROM dbo.Bike AS b
                        LEFT JOIN dbo.Owner AS o ON o.Id = b.OwnerId
                        LEFT JOIN dbo.BikeType AS bt ON bt.Id = b.BikeTypeId
                        LEFT JOIN dbo.WorkOrder AS wo ON b.Id = wo.BikeId
                        WHERE b.Id = @Id


                    ";

                    cmd.Parameters.AddWithValue("@Id", id);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        Bike bike = null;

                        while (reader.Read())
                        {
                            if (bike == null && !reader.IsDBNull(reader.GetOrdinal("BikeId")))
                            {
                                bike = new Bike()
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("BikeId")),
                                    Brand = reader.GetString(reader.GetOrdinal("Brand")),
                                    Color = reader.GetString(reader.GetOrdinal("Color")),
                                    Owner = new Owner()
                                    {
                                        Id = reader.GetInt32(reader.GetOrdinal("OwnerId")),
                                        Name = reader.GetString(reader.GetOrdinal("OwnerName")),
                                        Address = reader.GetString(reader.GetOrdinal("Address"))
                                    },
                                    BikeType = new BikeType()
                                    {
                                        Id = reader.GetInt32(reader.GetOrdinal("BikeTypeId")),
                                        Name = reader.GetString(reader.GetOrdinal("BikeTypeName"))
                                    },
                                    WorkOrders = new List<WorkOrder>()
                                };

                                if (!reader.IsDBNull(reader.GetOrdinal("WorkOrderId")))
                                {
                                    WorkOrder workOrder = new()
                                    {
                                        Id = reader.GetInt32(reader.GetOrdinal("WorkOrderId")),
                                        DateInitiated = reader.GetDateTime(reader.GetOrdinal("DateInitiated")),
                                        Description = reader.GetString(reader.GetOrdinal("Description")),
                                    };

                                    if (!reader.IsDBNull(reader.GetOrdinal("DateCompleted")))
                                    {
                                        workOrder.DateCompleted = reader.GetDateTime(reader.GetOrdinal("DateCompleted"));
                                    }    

                                    bike.WorkOrders.Add(workOrder);
                                }
                            }
                        }
                        
                        return bike;
                    }
                }
            }
        }

        public int GetBikesInShopCount()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT COUNT(DISTINCT BikeId)
                        FROM dbo.WorkOrder
			WHERE DateCompleted IS NULL
                    ";

                    return (int)cmd.ExecuteScalar();
                }
            }
        }
    }
}
