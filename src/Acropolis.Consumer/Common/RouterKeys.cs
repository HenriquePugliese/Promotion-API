namespace Acropolis.Consumer.Common;

public static class RouterKeys
{
    public const string CustomerCreated = "customer.customer.created";
    public const string CustomerChanged = "customer.customer.changed";
    public const string CustomerRemoved = "customer.customer.removed";

    public const string GroupCustomerCreated = "acropolis.customer.created";
    public const string GroupCustomerChanged = "acropolis.customer.changed";
    public const string GroupCustomerRemoved = "acropolis.customer.removed";

    public const string CatalogProductCreated = "catalog.product.created";
    public const string CatalogProductUpdated = "catalog.product.updated";
    public const string CatalogProductChanged = "catalog.product.changed";
    public const string CatalogProductRemoved = "catalog.product.removed";

    public const string GroupProductCreated = "acropolis.product.created";
    public const string GroupProductUpdated = "acropolis.product.updated";
    public const string GroupProductChanged = "acropolis.product.changed";
    public const string GroupProductRemoved = "acropolis.product.removed";

    public const string CatalogAttributeUpdated = "catalog.attribute.updated";

    public const string GroupCatalogAttributeUpdated = "acropolis.attribute.updated";
}