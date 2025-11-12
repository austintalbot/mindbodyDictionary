variable "environment" {
  type = string
}

variable "location" {
  type = string
}

variable "project_name" {
  type = string
}

variable "resource_group_name" {
  type = string
}

variable "storage_account_name" {
  type = string
}

variable "storage_account_key" {
  type      = string
  sensitive = true
}

variable "cosmosdb_endpoint" {
  type      = string
  sensitive = true
}

variable "cosmosdb_key" {
  type      = string
  sensitive = true
}

variable "cosmosdb_database_name" {
  type = string
}

variable "tags" {
  type = map(string)
}
