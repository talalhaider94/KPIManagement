using Microsoft.EntityFrameworkCore;
using Quantis.WorkFlow.APIBase.Framework;
using Quantis.WorkFlow.Models.Dashboard;
using Quantis.WorkFlow.Services.API;
using Quantis.WorkFlow.Services.DTOs.Dashboard;
using Quantis.WorkFlow.Services.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Quantis.WorkFlow.APIBase.API
{
    public class DashboardService : IDashboardService
    {
        private readonly WorkFlowPostgreSqlContext _dbcontext;
        private readonly IMappingService<DashboardDTO, DB_Dashboard> _dashboardMapper;
        private readonly IMappingService<Services.DTOs.Dashboard.WidgetDTO, DB_Widget> _widgetMapper;
        private readonly IMappingService<DashboardWidgetDTO, DB_DashboardWidget> _dashboardWidgetMapper;

        public DashboardService(WorkFlowPostgreSqlContext dbcontext,
            IMappingService<DashboardDTO, DB_Dashboard> dashboardMapper,
            IMappingService<Services.DTOs.Dashboard.WidgetDTO, DB_Widget> widgetMapper,
            IMappingService<DashboardWidgetDTO, DB_DashboardWidget> dashboardWidgetMapper
            )
        {
            _dbcontext = dbcontext;
            _dashboardMapper = dashboardMapper;
            _widgetMapper = widgetMapper;
            _dashboardWidgetMapper = dashboardWidgetMapper;
        }

        public List<DashboardDTO> GetDashboards(int userId)
        {
            try
            {
                var entities = _dbcontext.DB_Dashboards.Where(o => o.UserId == userId).Include(o => o.User).OrderBy(o => o.Name).ToList();
                return _dashboardMapper.GetDTOs(entities);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void ActivateDashboard(int id)
        {
            try
            {
                var dash = _dbcontext.DB_Dashboards.Single(o => o.Id == id);
                dash.IsActive = true;
                _dbcontext.SaveChanges();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public int GetDefaultDashboardId(int userId)
        {
            try
            {
                var dash = _dbcontext.DB_Dashboards.FirstOrDefault(o => o.UserId == userId && o.IsDefault);
                if (dash == null)
                {
                    return -1;
                }
                else
                {
                    return dash.Id;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void DeleteDashboard(int Id)
        {
            var dashboard= _dbcontext.DB_Dashboards.FirstOrDefault(o => o.Id==Id);
            var widgets = _dbcontext.DB_DashboardWidgets.Include(o=>o.DashboardWidgetSettings).Where(o => o.DashboardId == Id);
            var settings = widgets.SelectMany(o => o.DashboardWidgetSettings);
            _dbcontext.DB_DashboardWidgetSettings.RemoveRange(settings);
            _dbcontext.SaveChanges();
            _dbcontext.DB_DashboardWidgets.RemoveRange(widgets);
            _dbcontext.SaveChanges();
            _dbcontext.DB_Dashboards.Remove(dashboard);
            _dbcontext.SaveChanges();

        }
        

        public void SetDefaultDashboard(int id, int userId)
        {
            try
            {
                var dashnew = _dbcontext.DB_Dashboards.FirstOrDefault(o => o.Id == id && o.UserId == userId);
                var dash = _dbcontext.DB_Dashboards.FirstOrDefault(o => o.UserId == userId && o.IsDefault);
                if (dash != null && dashnew != null)
                {
                    dash.IsDefault = false;
                    _dbcontext.SaveChanges();
                }
                if (dashnew != null)
                {
                    dashnew.IsDefault = true;
                    _dbcontext.SaveChanges();
                }
                var lp = _dbcontext.UserLandingPages.FirstOrDefault(o => o.user_id == userId);
                if (lp != null)
                {
                    lp.selected_landingpage = false;
                    _dbcontext.SaveChanges();
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void DeactivateDashboard(int id)
        {
            try
            {
                var dash = _dbcontext.DB_Dashboards.Single(o => o.Id == id);
                dash.IsActive = false;
                _dbcontext.SaveChanges();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public int AddUpdateDasboard(DashboardDetailDTO dto, int userId)
        {
            try
            {
                if (dto.Id == 0)
                {
                    List<DB_DashboardWidget> dbwidgets = new List<DB_DashboardWidget>();
                    foreach (var dbw in dto.DashboardWidgets)
                    {
                        var ent = new DB_DashboardWidget();
                        ent = _dashboardWidgetMapper.GetEntity(dbw, ent);
                        dbwidgets.Add(ent);
                    }
                    DB_Dashboard entdb = new DB_Dashboard();
                    entdb.Name = dto.Name;
                    entdb.GlobalFilterId = dto.GlobalFilterId;
                    entdb.DashboardWidgets = dbwidgets;
                    entdb.UserId = userId;
                    entdb.CreatedOn=DateTime.Now;
                    entdb.ModifiedOn=DateTime.Now;

                    _dbcontext.DB_Dashboards.Add(entdb);
                    _dbcontext.SaveChanges();
                    return entdb.Id;
                }
                else
                {
                    var dashboardWidgetIds = dto.DashboardWidgets.Where(o => o.Id != 0).Select(o => o.Id).ToList();
                    var deletewidgets = _dbcontext.DB_DashboardWidgets.Include(o => o.DashboardWidgetSettings).Where(o => o.DashboardId == dto.Id && !dashboardWidgetIds.Contains(o.Id)).ToArray();
                    if (deletewidgets.Any())
                    {
                        foreach (var w in deletewidgets)
                        {
                            _dbcontext.DB_DashboardWidgetSettings.RemoveRange(w.DashboardWidgetSettings.ToArray());
                        }
                        _dbcontext.DB_DashboardWidgets.RemoveRange(deletewidgets);
                    }

                    var newwidgets = dto.DashboardWidgets.Where(o => o.Id == 0).ToList();
                    List<DB_DashboardWidget> dbwidgets = new List<DB_DashboardWidget>();
                    foreach (var dbw in newwidgets)
                    {
                        var ent = new DB_DashboardWidget();
                        ent = _dashboardWidgetMapper.GetEntity(dbw, ent);
                        ent.DashboardId = dto.Id;
                        dbwidgets.Add(ent);
                    }
                    _dbcontext.DB_DashboardWidgets.AddRange(dbwidgets.ToArray());
                    _dbcontext.SaveChanges();

                    var oldwidgets = dto.DashboardWidgets.Where(o => o.Id != 0).ToList();
                    foreach (var dbw in oldwidgets)
                    {
                        var ent = _dbcontext.DB_DashboardWidgets.Single(o => o.Id == dbw.Id);
                        ent = _dashboardWidgetMapper.GetEntity(dbw, ent);
                    }
                    var dashboard = _dbcontext.DB_Dashboards.Single(o => o.Id == dto.Id);
                    dashboard.GlobalFilterId = dto.GlobalFilterId;
                    dashboard.ModifiedOn=DateTime.Now;
                    dashboard.Name = dto.Name;
                    _dbcontext.SaveChanges();
                    return dto.Id;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public List<WidgetDTO> GetAllWidgets()
        {
            try
            {
                var entities = _dbcontext.DB_Widgets.Where(o => o.IsActive == true).ToList();
                return _widgetMapper.GetDTOs(entities);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public DashboardDetailDTO GetDashboardWigetsByDashboardId(int id)
        {
            try
            {
                var db = _dbcontext.DB_Dashboards.Single(o => o.Id == id);
                var entities = _dbcontext.DB_DashboardWidgets.Include(o => o.Widget).Include(o => o.DashboardWidgetSettings).Where(o => o.DashboardId == id);
                var widgets = _dashboardWidgetMapper.GetDTOs(entities.ToList());
                return new DashboardDetailDTO()
                {
                    Id = db.Id,
                    Name = db.Name,
                    GlobalFilterId = db.GlobalFilterId,
                    DashboardWidgets = widgets
                };
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void SaveDashboardState(List<DashboardWidgetBaseDTO> dtos)
        {
            foreach (var dto in dtos)
            {
                foreach (var p in dto.Properties)
                {
                    var prop = _dbcontext.DB_DashboardWidgetSettings.FirstOrDefault(r => r.DashboardWidgetId == dto.Id && r.SettingType == 0 && r.SettingKey == p.Key);
                    if (prop != null)
                    {
                        prop.SettingValue = p.Value;
                    }
                    else
                    {
                        var setting = new DB_DashboardWidgetSetting()
                        {
                            SettingKey = p.Key,
                            SettingType = 0,
                            SettingValue = p.Value,
                            DashboardWidgetId = dto.Id
                        };
                        _dbcontext.DB_DashboardWidgetSettings.Add(setting);
                    }
                }
                foreach (var p in dto.Filters)
                {
                    var prop = _dbcontext.DB_DashboardWidgetSettings.FirstOrDefault(r => r.DashboardWidgetId == dto.Id && r.SettingType == 1 && r.SettingKey == p.Key);
                    if (prop != null)
                    {
                        prop.SettingValue = p.Value;
                    }
                    else
                    {
                        var setting = new DB_DashboardWidgetSetting()
                        {
                            SettingKey = p.Key,
                            SettingType = 1,
                            SettingValue = p.Value,
                            DashboardWidgetId = dto.Id
                        };
                        _dbcontext.DB_DashboardWidgetSettings.Add(setting);
                    }
                }
                _dbcontext.SaveChanges();
            }
            if (dtos.Any())
            {
                var dashboardId=_dbcontext.DB_DashboardWidgets.FirstOrDefault(o => o.Id == dtos.FirstOrDefault().Id).DashboardId;
                var dashboard=_dbcontext.DB_Dashboards.FirstOrDefault(o => o.Id == dashboardId);
                dashboard.ModifiedOn = DateTime.Now;
                _dbcontext.SaveChanges();
            }
        }
    }
}