import { INameCounter } from "../dto/INameCounter";

const StatistricsBlock = (props: {list: INameCounter[], totalCount: number, title: string}) => {
    const colors: string[] = ["#AC92EB", "#4CF1E8", "#A0D568", "#FFCE54", "#ED5564", "#d9d9d9"];
    const filteredList = props.list
        .sort((a, b) => b.count - a.count)
        .filter(item => item.name !== "Other");
    const otherCount = props.list.find(item => item.name === "Other")?.count || 0;

    return (
        <>
            <h4>{props.title}</h4>
            <div className="stats-box">
                <StatistricsBar list={filteredList} colors={colors} totalCount={props.totalCount} otherCount={otherCount} />
                <StatisticsLegend colors={colors} list={filteredList} otherCount={otherCount} />
            </div>
        </>
    )
}

const StatistricsBar = (props: {list: INameCounter[], totalCount: number, colors: string[], otherCount: number}) => {
    return (
        <div className="stats-outer-bar">
            {
                props.list
                .map((key, index) => {
                    return (
                        <div key={key.name} className="stats-inner-bar" 
                            style={{"width": (key.count / props.totalCount * 100) + "%", "backgroundColor": props.colors[index]}}>
                        </div>
                    )
                })
            }
            {
                props.otherCount !== 0 && 
                <div className="stats-inner-bar" style={{"width": (props.otherCount / props.totalCount * 100) + "%", "background": props.colors[5]}}></div>
            }
        </div>
    )
}

const StatisticsLegend = (props: {list: INameCounter[], colors: string[], otherCount: number}) => {
    return (
        <div className="stats-legend-outer">
            {
                props.list.map((key, index) => {
                    return (
                        <div key={key.name} className="legend-inner">
                            <div className="legend-box" style={{"backgroundColor": props.colors[index]}}></div>
                            <div>{key.name}</div>
                        </div>
                    )
                })
            }
            {
                props.otherCount !== 0 && 
                <div className="legend-inner">
                    <div className="legend-box" style={{"backgroundColor": props.colors[5]}}></div>
                    <div>Other</div>
                </div>
            }
        </div>
    )
}


export default StatistricsBlock;