namespace NetBattle.Field {
    public interface IFieldEventHandler {
        bool Handle(FieldEvent evt);
    }
}