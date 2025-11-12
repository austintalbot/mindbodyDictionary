# Monitoring Module

Creates Application Insights and Log Analytics workspace for centralized monitoring and logging.

## Features

- Application Insights for application telemetry
- Log Analytics workspace for centralized logs
- Configurable log retention
- Workspace-based Application Insights
- Automatic integration

## Usage

```hcl
module "monitoring" {
  source = "./modules/monitoring"

  project_name        = "mbd"
  environment         = "dev"
  resource_group_name = "rg-mbd-dev"
  location            = "East US"
  
  log_retention_days = 30
  
  tags = {
    Component = "Monitoring"
  }
}
```

## Inputs

| Name | Description | Type | Default | Required |
|------|-------------|------|---------|----------|
| project_name | Name of the project | string | n/a | yes |
| environment | Environment name | string | n/a | yes |
| resource_group_name | Name of the resource group | string | n/a | yes |
| location | Azure region | string | n/a | yes |
| log_analytics_sku | SKU for Log Analytics | string | "PerGB2018" | no |
| log_retention_days | Log retention in days (30-730) | number | 30 | no |
| tags | Tags to apply | map(string) | {} | no |

## Outputs

| Name | Description |
|------|-------------|
| log_analytics_workspace_id | ID of Log Analytics workspace |
| log_analytics_workspace_name | Name of Log Analytics workspace |
| application_insights_id | ID of Application Insights |
| application_insights_name | Name of Application Insights |
| application_insights_instrumentation_key | Instrumentation key (sensitive) |
| application_insights_connection_string | Connection string (sensitive) |

## Log Analytics SKUs

| SKU | Description | Pricing Model |
|-----|-------------|---------------|
| PerGB2018 | Pay-as-you-go per GB | $2.30/GB (first 5GB free) |
| CapacityReservation | Commitment tier | Discounted for >100GB/day |
| Free | Limited free tier | 500MB/day limit |

## Retention Periods

| Environment | Recommended Retention | Cost Impact |
|-------------|---------------------|-------------|
| Development | 30 days | Low |
| Staging | 60 days | Medium |
| Production | 90-180 days | High |
| Compliance | 365+ days | Very high |

## Examples

### Development Environment (Cost-Optimized)
```hcl
module "monitoring_dev" {
  source = "./modules/monitoring"
  
  project_name        = "app"
  environment         = "dev"
  resource_group_name = "rg-app-dev"
  location            = "East US"
  
  log_analytics_sku  = "PerGB2018"
  log_retention_days = 30
}
```

### Production Environment (Extended Retention)
```hcl
module "monitoring_prod" {
  source = "./modules/monitoring"
  
  project_name        = "app"
  environment         = "prod"
  resource_group_name = "rg-app-prod"
  location            = "East US"
  
  log_analytics_sku  = "PerGB2018"
  log_retention_days = 90
  
  tags = {
    Compliance = "SOC2"
    CostCenter = "Engineering"
  }
}
```

## Integration with Function Apps

Use the connection string output when configuring Function Apps:

```hcl
module "monitoring" {
  source = "./modules/monitoring"
  # ... configuration
}

module "function_app" {
  source = "./modules/function_app"
  
  app_insights_connection_string = module.monitoring.application_insights_connection_string
  # ... other configuration
}
```

## Querying Logs

### Common Kusto Queries

**Function App Errors:**
```kusto
traces
| where severityLevel >= 3
| project timestamp, message, severityLevel
| order by timestamp desc
```

**Performance Issues:**
```kusto
requests
| where duration > 1000
| summarize count() by bin(timestamp, 1h)
```

**Custom Events:**
```kusto
customEvents
| where name == "NotificationSent"
| summarize count() by bin(timestamp, 1h)
```

## Alerts Configuration

After deployment, consider setting up alerts for:

1. **Error Rate** - When error rate > 5%
2. **Response Time** - When P95 > 2 seconds
3. **Availability** - When availability < 99%
4. **Log Ingestion** - When log volume spikes
5. **Cost** - When daily cost exceeds threshold

## Cost Optimization

1. **Filter logs** - Don't log verbose information in production
2. **Sampling** - Use adaptive sampling for high-volume apps
3. **Archive old data** - Export to cheap storage after 30 days
4. **Use retention policies** - Don't keep data longer than needed
5. **Monitor costs** - Set up cost alerts

## References

- [Application Insights Overview](https://learn.microsoft.com/en-us/azure/azure-monitor/app/app-insights-overview)
- [Log Analytics Workspace](https://learn.microsoft.com/en-us/azure/azure-monitor/logs/log-analytics-workspace-overview)
- [Kusto Query Language](https://learn.microsoft.com/en-us/azure/data-explorer/kusto/query/)
