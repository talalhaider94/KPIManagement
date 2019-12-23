export default class WorkFlowHelper {
    static formatSummary(summary) {
        if (!!summary && summary.length > 0) {
            let splitSummary = summary.trim().split('|');
            if (splitSummary.length > 0) {
                return splitSummary;
            } else {
                return [summary];
            }
        } else {
            return ['N/A'];
        }
    }

    static formatDescription(description) {
        const regex = /([\w]+:)("(([^"])*)"|'(([^'])*)'|(([^\s])*))/g;
        if (!!description && description.length > 0) {
            let stringMatches = description.match(regex);
            if (stringMatches && stringMatches.length > 0) {
                return stringMatches.map((key, index) => {
                    return {
                        key,
                        value: description.split(stringMatches[index]).pop().split(stringMatches[index + 1])[0]
                    }
                });
            } else {
                return [description];
            }
        } else {
            return ['N/A'];
        }
    }

    static getDescriptionField(description, field) {
        return this.formatDescription(description).find(column => column.key === field);
    }

    static formatCheckBoxColumn() {
        return `Verde -> KPI Compliant, Rosso -> KPI Non Compliant, Grigio -> KPI Non Calcolato`.trim().split(',');
    }
}
