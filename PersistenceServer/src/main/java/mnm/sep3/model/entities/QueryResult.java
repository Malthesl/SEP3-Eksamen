package mnm.sep3.model.entities;

import java.util.List;

public class QueryResult<T> {
    public List<T> results;
    public int count;

    public QueryResult(List<T> results, int count) {
        this.results = results;
        this.count = count;
    }
}
