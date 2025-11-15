  private static readonly Lazy<nint> _$NAME$Offset = new(() => Schema.GetOffset($HASH$), LazyThreadSafetyMode.None);

  public $INTERFACE_TYPE$ $NAME$ {
    get => new $IMPL_TYPE$(_Handle + _$NAME$Offset.Value);
  }